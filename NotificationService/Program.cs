
using Asp.Versioning;
using ECommerce.Common.Clients;
using ECommerce.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NotificationService.Configurations;
using NotificationService.ExternalServices;
using NotificationService.Infrastructure.Database;
using NotificationService.Repositories;
using NotificationService.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text.Json.Serialization;
using LoggerFactory = ECommerce.Common.Infrastructures.Serilog.LoggerFactory;

namespace NotificationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        var serviceConfiguration = new ServiceConfiguration();
        builder.Configuration.Bind(serviceConfiguration);
        builder.Services.AddSingleton<IServiceConfiguration>(serviceConfiguration);

        Log.Logger = LoggerFactory.CreateLogger(builder.Configuration);
        builder.Host.UseSerilog();
        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(serviceConfiguration.ConnectionStrings.NotificationService));

        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
        builder.Services.AddScoped<IExternalEmailProvider, ExternalEmailProvider>();

        builder.Services.AddECommerceCommon();
        builder.Services.AddClientFactory(builder.Configuration);

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version")
            );
        });

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
            httpClientBuilder.AddScopedPolicyHandler((_) => GetCircuitBreakerPolicy(policySettings.CircuitBreaker));
            noRetryHttpClientBuilder.AddScopedPolicyHandler((_) => GetCircuitBreakerPolicy(policySettings.CircuitBreaker));
        }

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName); // includes namespace!
        });
        builder.Services.AddHealthChecks(serviceConfiguration.ConnectionStrings.NotificationService);

        var app = builder.Build();

        // Apply pending migrations and seed data
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            dbContext.Database.Migrate();
        }

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
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
}
