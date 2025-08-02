using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using Serilog;
using System.Diagnostics;

namespace ECommerce.Common.Infrastructures.HttpPolicies
{
    public class CommonHttpPolicyFactory : ICommonHttpPolicyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConcurrentPolicyRegistry<string> _registryPolicy;

        public CommonHttpPolicyFactory(IServiceProvider serviceProvider, IConcurrentPolicyRegistry<string> registryPolicy)
        {
            _serviceProvider = serviceProvider;
            _registryPolicy = registryPolicy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetPolicy(HttpRequestMessage request, Func<string, IAsyncPolicy<HttpResponseMessage>> policyFactory, Func<HttpRequestMessage, string> keySelector)
        {
            var stopwatch = Stopwatch.StartNew();

            var key = keySelector(request);

            if (_registryPolicy.TryGet<IAsyncPolicy<HttpResponseMessage>>(key, out var policy))
            {
                return policy;
            }

            var newPolicy = policyFactory(key).WithPolicyKey(key);
            _registryPolicy.TryAdd(key, newPolicy);

            stopwatch.Stop();

            Log.Information("CircuitBreaker policy key '{Key}' is created for {Method} {ApiRoute} in {Duration} ticks.",
                key, request.Method.Method, request.RequestUri?.AbsolutePath, stopwatch.ElapsedTicks);

            Log.Debug("CircuitBreaker policy {@Policy} is created for {Method} {ApiRoute} in {Duration} ticks.",
                policy, request.Method.Method, request.RequestUri?.AbsolutePath, stopwatch.ElapsedTicks);

            return newPolicy;
        }

        public string BuildScopedEndpointPolicyKey(HttpRequestMessage request)
        {
            string endpointMethod;
            string endpointRoute;

            try
            {
                var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext == null)
                {
                    throw new ArgumentException(nameof(httpContext));
                }

                endpointMethod = httpContext.Request.Method;

                var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint as RouteEndpoint;

                endpointRoute = endpoint?.RoutePattern.RawText ?? string.Empty;

                return $"ep-{endpointMethod}-{endpointRoute}".ToLower();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while building CircuitBreaker key for {Method} request to {URL}",
                    request.Method.Method, request.RequestUri?.AbsolutePath);
                throw;
            }
        }
    }
}
                                                                                                                                                                                                                                                   