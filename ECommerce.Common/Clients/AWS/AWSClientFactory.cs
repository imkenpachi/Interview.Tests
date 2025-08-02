using Amazon;
using Amazon.SQS;
using ECommerce.Common.Infrastructures.Tracers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients.AWS
{
    public class AWSClientFactory : BaseAWSCredentialCacheAdapter, IClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;
        private readonly ILocalCorrelationIdAccessor _localCorrelationIdAccessor;

        public AWSClientFactory(IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
            , ICorrelationIdGenerator correlationIdGenerator
            , ILocalCorrelationIdAccessor localCorrelationIdAccessor
            , IMemoryCache memoryCache) : base(memoryCache)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _correlationIdGenerator = correlationIdGenerator;
            _localCorrelationIdAccessor = localCorrelationIdAccessor;
        }

        public IMessageQueueClient GetQueueClient(string connectionStringKey, string queueName)
        {
            var queueGroup = _configuration["ConnectionStrings:QueueGroup"] ?? queueName;
            return GetSQSClient(connectionStringKey, queueName, queueGroup);
        }

        private SQSMessageQueueClient GetSQSClient(string connectionStringKey, string queueName, string queueGroup)
        {
            var url = _configuration[connectionStringKey];

            var region = _configuration["ConnectionStrings:AWSRegion"];

            var amazonSQSConfig = string.IsNullOrEmpty(region) ? new AmazonSQSConfig { ServiceURL = url } : new AmazonSQSConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(region) };

            var amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);

            var sqsMessageQueueClient = new SQSMessageQueueClient(amazonSQSClient, 
                url, 
                queueName, 
                queueGroup, 
                _httpContextAccessor,
                _correlationIdGenerator, 
                _localCorrelationIdAccessor);

            if (int.TryParse(_configuration["ConnectionStrings:SQS:ReceiveRequestWaitTimeSeconds"], out int waitTimeSeconds))
            {
                Log.Debug("ReceiveRequestWaitTimeSeconds changed from {OldValue} to {NewValue}", sqsMessageQueueClient.ReceiveRequestWaitTimeSeconds, waitTimeSeconds);
                sqsMessageQueueClient.ReceiveRequestWaitTimeSeconds = waitTimeSeconds;
            }
            if (int.TryParse(_configuration["ConnectionStrings:SQS:ReceiveRequestVisibilityTimeoutSeconds"], out int visibilityTimeout))
            {
                Log.Debug("ReceiveRequestVisibilityTimeoutSeconds changed from {OldValue} to {NewValue}", sqsMessageQueueClient.ReceiveRequestVisibilityTimeoutSeconds, visibilityTimeout);
                sqsMessageQueueClient.ReceiveRequestVisibilityTimeoutSeconds = visibilityTimeout;
            }
            if (int.TryParse(_configuration["ConnectionStrings:SQS:ReceiveRequestMaxNumberOfMessages"], out int maxNumberOfMessages))
            {
                Log.Debug("ReceiveRequestMaxNumberOfMessages changed from {OldValue} to {NewValue}", sqsMessageQueueClient.ReceiveRequestMaxNumberOfMessages, maxNumberOfMessages);
                sqsMessageQueueClient.ReceiveRequestMaxNumberOfMessages = maxNumberOfMessages;
            }
            if (int.TryParse(_configuration["ConnectionStrings:SQS:SendRequestDelaySeconds"], out int requestDelaySeconds))
            {
                Log.Debug("SendRequestDelaySeconds changed from {OldValue} to {NewValue}", sqsMessageQueueClient.SendRequestDelaySeconds, requestDelaySeconds);
                sqsMessageQueueClient.SendRequestDelaySeconds = requestDelaySeconds;
            }
            if (bool.TryParse(_configuration["ConnectionStrings:SQS:EnableMessageDeduplicationId"], out bool enableMessageDeduplicationId))
            {
                Log.Debug("EnableMessageDeduplicationId changed from {OldValue} to {NewValue}", sqsMessageQueueClient.EnableMessageDeduplicationId, enableMessageDeduplicationId);
                sqsMessageQueueClient.EnableMessageDeduplicationId = enableMessageDeduplicationId;
            }

            return sqsMessageQueueClient;
        }
    }
}
