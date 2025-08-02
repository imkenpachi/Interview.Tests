using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients.AWS
{
    public abstract class BaseAWSCredentialCacheAdapter
    {
        private readonly IAsyncPolicy<AssumeRoleResponse> _retryPolicy;
        protected readonly IMemoryCache _memoryCache;

        protected BaseAWSCredentialCacheAdapter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            _retryPolicy = Policy<AssumeRoleResponse>.Handle<Exception>()
                .RetryAsync(3, (exception, count) =>
                {
                    Log.Warning("Fail to fetch IAM role, retry attemp {Counter} out of 3", count);
                });
        }

        public async Task<AssumeRoleResponse?> InvokeCredentialFromCacheAsync(RegionEndpoint regionEndpoint, string sessionName, string roleArn)
        {
            if (_memoryCache.TryGetValue(sessionName, out AssumeRoleResponse? memCachedValue))
            {
                Log.Information("Fetching IAM role for session {SessionName} from memory cache, with result {IsSuccess}.", sessionName, memCachedValue != null);
            }
            else
            {
                Log.Information("Cannot fetch IAM role for session {SessionName} from memory cache, re-send assume role request and renew cache", sessionName);
                try
                {
                    var awsClient = new AmazonSecurityTokenServiceClient(regionEndpoint);

                    var assumeRoleRequest = new AssumeRoleRequest
                    {
                        RoleArn = roleArn,
                        RoleSessionName = sessionName,
                    };

                    memCachedValue = await _retryPolicy.ExecuteAsync(() => awsClient.AssumeRoleAsync(assumeRoleRequest));

                    var sessionDuration = memCachedValue.Credentials.Expiration.Value.TimeOfDay.TotalSeconds;

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(sessionDuration - 10)) //offset 10s
                        .SetPriority(CacheItemPriority.Normal);

                    Log.Information("Setting new IAM response to memory cache for session {SessionName}, with value {@Response}.", sessionName, memCachedValue);

                    _memoryCache.Set(sessionName, memCachedValue, cacheEntryOptions);
                }
                catch (Exception ex)
                {
                    Log.Error(ex,"Cannot retrieve AWS credentials from cache nor invoke new security token");
                    throw;
                }
            }

            return memCachedValue;
        }

    }
}
