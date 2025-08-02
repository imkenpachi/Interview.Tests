using ECommerce.Common.Clients;
using ECommerce.Common.Extensions;
using InvoiceListener.Configurations;
using InvoiceListener.ExternalServices;
using InvoiceListener.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using LoggerFactory = ECommerce.Common.Infrastructures.Serilog.LoggerFactory;

namespace InvoiceListener;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        Log.Logger = LoggerFactory.CreateLogger(builder.Configuration);
        builder.Logging.AddSerilog();

        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        var serviceConfiguration = new ServiceConfiguration();
        builder.Configuration.Bind(serviceConfiguration);

        builder.Services.AddSingleton<IServiceConfiguration>(serviceConfiguration);
        builder.Services.AddScoped<IInvoiceGenerationHandlingService, InvoiceGenerationHandlingService>();
        builder.Services.AddScoped<IExternalInvoiceService, ExternalInvoiceService>();
        builder.Services.AddScoped<IExternalOrderService, ExternalOrderService>();
        builder.Services.AddECommerceCommon();
        builder.Services.AddClientFactory(builder.Configuration);

        var noRetryHttpClientBuilder = builder.Services.AddHttpClient<INoRetryHttpClientWrapper, NoRetryHttpClientWrapper>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddCorrelationIdMessageHandler();

        var httpClientBuilder = builder.Services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddCorrelationIdMessageHandler();

        var policySettings = serviceConfiguration.PolicySettings;

        if (policySettings != null && policySettings.Retry.EnabledAsBoolean)
        {
            httpClientBuilder.AddPolicyHandler(GetRetryPolicy(policySettings.Retry));
        }

        if (policySettings != null && policySettings.CircuitBreaker.EnabledAsBoolean)
        {
            httpClientBuilder.AddScopedPolicyHandler((_) => GetCircuitBreakerPolicy(policySettings.CircuitBreaker), (request) => GetCircuitBreakerPolicyKey(request));
            noRetryHttpClientBuilder.AddScopedPolicyHandler((_) => GetCircuitBreakerPolicy(policySettings.CircuitBreaker), (request) => GetCircuitBreakerPolicyKey(request));
        }

        var host = builder.Build();
        host.Run();
    }
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(RetryPolicySettings policySettings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => policySettings.ForStatusCodesAsArray.Contains((int)response.StatusCode))
            .WaitAndRetryAsync(
                policySettings.CountAsInteger,
                attemp => TimeSpan.FromMilliseconds(attemp * policySettings.SleepDurationInMillisecondsAsInteger)
            );
    }
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(CircuitBreakerPolicySettings circuitBreakerSettings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => circuitBreakerSettings.ForStatusCodesAsArray.Contains((int)response.StatusCode))
            .AdvancedCircuitBreakerAsync(
                failureThreshold: circuitBreakerSettings.FailureThresholdAsDouble,
                samplingDuration: TimeSpan.FromMilliseconds(circuitBreakerSettings.SamplingDurationInMillisecondsAsInt),
                minimumThroughput: circuitBreakerSettings.MinimumThroughputAsInt,
                durationOfBreak: TimeSpan.FromMilliseconds(circuitBreakerSettings.DurationOfBreakInMillisecondsAsInt),
                onBreak: (handledFault, duration, context) =>
                {
                    Log.Warning("Response for {Request} is {Response} for many times. Break it for {Duration} seconds.",
                    handledFault?.Result?.RequestMessage?.RequestUri?.AbsoluteUri,
                    handledFault?.Result?.StatusCode,
                    duration.TotalSeconds
                    );
                },
                onReset: (context) =>
                {
                    Log.Information("CircuitBreaker is reset.");
                }
            );
    }
    private static string GetCircuitBreakerPolicyKey(HttpRequestMessage request)
    {
        string endpointMethod = request.Method.Method;
        string endpointRoute = request.RequestUri?.AbsolutePath ?? string.Empty;

        return $"ep-{endpointMethod}-{endpointRoute}".ToLower();
    }
}