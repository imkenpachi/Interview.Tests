using ECommerce.Common.Clients;
using ECommerce.Common.Clients.AWS;
using ECommerce.Common.Infrastructures.HttpPolicies;
using ECommerce.Common.Infrastructures.Tracers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ECommerce.Common.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register the required common services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddECommerceCommon(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ICommonHttpPolicyFactory, CommonHttpPolicyFactory>();
            services.AddPolicyRegistry();
            services.AddMemoryCache();
            services.AddCorrelationId();

            return services;
        }

        /// <summary>
        /// Register ClientFactory (currently only support AWS)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            //can conditional check Cloud platform if more than one Cloud provider can be supported.
            services.TryAddScoped<IClientFactory, AWSClientFactory>();

            return services;
        }

        /// <summary>
        /// Adds services required for managing correlation IDs to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method registers the necessary services for generating, accessing, and managing
        /// correlation IDs in an application. It includes support for HTTP context-based correlation ID
        /// management.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the correlation ID services are added.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance, allowing for method chaining.</returns>
        public static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<ILocalCorrelationIdAccessor, LocalCorrelationIdAccessor>();
            services.TryAddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
            services.TryAddScoped<ICorrelationIdManager, CorrelationIdManager>();

            return services;
        }

        /// <summary>
        /// Adds health checks to the service collection, including a database health check if a connection string is
        /// provided.
        /// </summary>
        /// <remarks>If a connection string named "DefaultConnection" is present in the configuration, a
        /// SQL Server health check is added with the name "production-db-check" and the tags "db", "sql", and "MS SQL
        /// Server".</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the health checks will be added.</param>
        /// <param name="databaseConnectionString">The database connection string for the health check.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with health checks configured.</returns>
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, string databaseConnectionString)
        {
            var healthCheckBuilder = services.AddHealthChecks(); // Correctly obtain the IHealthChecksBuilder
            const string dbHealthCheckName = "db-check";

            if (!string.IsNullOrEmpty(databaseConnectionString))
            {
                healthCheckBuilder.AddSqlServer(
                    connectionString: databaseConnectionString,
                    name: dbHealthCheckName,
                    tags: ["db", "sql", "MS SQL Server"] // Correctly use an array for tags
                );
            }

            return services;
        }
    }
}
