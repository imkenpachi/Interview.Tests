using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace ECommerce.Common.Infrastructures.Serilog
{
    public static class LoggerFactory
    {
        
        /// <summary>
        /// Creates and configures a Serilog <see cref="ILogger"/> instance using the provided <see cref="IConfiguration"/>.
        /// The logger is enriched with context, custom destructuring for exceptions and HTTP messages,
        /// and outputs logs to the console in Elasticsearch JSON format.
        /// </summary>
        /// <param name="configuration">The application configuration containing Serilog settings.</param>
        /// <returns>A configured <see cref="ILogger"/> instance.</returns>
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Destructure.ToMaximumDepth(5) // Limits object graph traversal depth
                .Destructure.ToMaximumCollectionCount(50) // Limits items from collections
                .Destructure.ToMaximumStringLength(1000) // Truncate long strings
                .Destructure.ByTransforming<Exception>(ex => new
                {
                    ex.Message,
                    ex.StackTrace,
                    ex.Source,
                    ex.HResult
                })
                .Destructure.ByTransforming<System.Net.Http.HttpRequestMessage>(req => new
                {
                    req.Method,
                    req.RequestUri,
                    Headers = req.Headers.ToString()
                })
                .Destructure.ByTransforming<System.Net.Http.HttpResponseMessage>(res => new
                {
                    res.StatusCode,
                    Headers = res.Headers.ToString(),
                    Content = res.Content?.ReadAsStringAsync().Result // use with caution!
                })
                .WriteTo.Console(new ElasticsearchJsonFormatter())
                .CreateLogger();
            Log.Logger = logger;
            return logger;
        }
    }
}
