using ECommerce.Common.Infrastructures.HttpPolicies;
using ECommerce.Common.Infrastructures.Tracers.HttpMessageHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;

namespace ECommerce.Common.Extensions
{
    public static class HttpClientBuilderExtension
    {
        /// <summary>
        /// Adds a scoped <see cref="PolicyHttpMessageHandler"/> to the HTTP client pipeline,  allowing dynamic
        /// selection of a resilience policy based on the request.
        /// </summary>
        /// <remarks>This method integrates a scoped <see cref="PolicyHttpMessageHandler"/> into the HTTP
        /// client pipeline.  The handler dynamically selects a resilience policy for each request based on the key
        /// provided by  <paramref name="keySelector"/> and the policy created by <paramref name="policyFactory"/>. 
        /// This is useful for scenarios where different requests require different resilience strategies.</remarks>
        /// <param name="builder">The <see cref="IHttpClientBuilder"/> to configure. Cannot be <see langword="null"/>.</param>
        /// <param name="policyFactory">A factory method that creates an <see cref="IAsyncPolicy{HttpResponseMessage}"/>  based on a policy key.
        /// Cannot be <see langword="null"/>.</param>
        /// <param name="keySelector">A function that extracts a policy key from the <see cref="HttpRequestMessage"/>.  This key is used to
        /// determine which policy to apply. Cannot be <see langword="null"/>.</param>
        /// <returns>The same <see cref="IHttpClientBuilder"/> instance, allowing further configuration chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/>, <paramref name="policyFactory"/>, or <paramref name="keySelector"/> is
        /// <see langword="null"/>.</exception>
        public static IHttpClientBuilder AddScopedPolicyHandler(this IHttpClientBuilder builder, Func<string, IAsyncPolicy<HttpResponseMessage>> policyFactory, Func<HttpRequestMessage, string> keySelector)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (policyFactory == null)
            {
                throw new ArgumentNullException(nameof(policyFactory));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            builder.AddHttpMessageHandler((serviceProvider) =>
            {
                var httpPolicyFactory = serviceProvider.GetRequiredService<ICommonHttpPolicyFactory>();

                return new PolicyHttpMessageHandler((requestMessage) => httpPolicyFactory.GetPolicy(requestMessage, policyFactory, keySelector));
            });

            return builder;
        }

        /// <summary>
        /// Adds a scoped policy handler to the HTTP client pipeline using the specified policy factory.
        /// </summary>
        /// <remarks>This method registers a <see cref="PolicyHttpMessageHandler"/> in the HTTP client
        /// pipeline. The handler applies a policy created by the provided <paramref name="policyFactory"/> for each
        /// HTTP request. The policy is scoped to the request context, enabling dynamic policy selection based on
        /// request-specific criteria.</remarks>
        /// <param name="builder">The <see cref="IHttpClientBuilder"/> to configure. Cannot be <see langword="null"/>.</param>
        /// <param name="policyFactory">A factory method that creates an <see cref="IAsyncPolicy{HttpResponseMessage}"/> based on a policy key. The
        /// key is derived from the HTTP request context. Cannot be <see langword="null"/>.</param>
        /// <returns>The <see cref="IHttpClientBuilder"/> instance, allowing further configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="policyFactory"/> is <see langword="null"/>.</exception>
        public static IHttpClientBuilder AddScopedPolicyHandler(this IHttpClientBuilder builder, Func<string, IAsyncPolicy<HttpResponseMessage>> policyFactory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (policyFactory == null)
            {
                throw new ArgumentNullException(nameof(policyFactory));
            }

            builder.AddHttpMessageHandler((serviceProvider) =>
            {
                var httpPolicyFactory = serviceProvider.GetRequiredService<ICommonHttpPolicyFactory>();

                return new PolicyHttpMessageHandler((requestMessage) => httpPolicyFactory.GetPolicy(requestMessage, policyFactory, httpPolicyFactory.BuildScopedEndpointPolicyKey));
            });

            return builder;
        }

        /// <summary>
        /// Adds a message handler to the HTTP client pipeline that includes a correlation ID in outgoing requests.
        /// </summary>
        /// <remarks>This method registers a <see cref="CorrelationIdMessageHandler"/> with the HTTP
        /// client, ensuring that a correlation ID is included in the headers of all outgoing HTTP requests. The
        /// correlation ID can be used for tracking and logging purposes across distributed systems.</remarks>
        /// <param name="builder">The <see cref="IHttpClientBuilder"/> to configure.</param>
        /// <returns>The <see cref="IHttpClientBuilder"/> instance, for chaining additional configurations.</returns>
        public static IHttpClientBuilder AddCorrelationIdMessageHandler(this IHttpClientBuilder builder)
        {
            builder.Services.TryAddTransient<CorrelationIdMessageHandler>();
            builder.AddHttpMessageHandler<CorrelationIdMessageHandler>();

            return builder;
        }
    }
}
