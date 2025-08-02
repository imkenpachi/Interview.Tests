using ECommerce.Common.Infrastructures.Tracers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommerce.Common.Helpers
{
    internal static class CorrelationIdHelpers
    {
        internal static CorrelationModel GetCorrelation(
            ILocalCorrelationIdAccessor localCorrelationIdAccessor,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationIdGenerator correlationIdGenerator)
        {
            return localCorrelationIdAccessor?.Correlation
                ?? GetCorrelationIdManager(httpContextAccessor)?.GetCorrelation()
                ?? correlationIdGenerator.GenerateNewCorrelation();
        }

        private static ICorrelationIdManager? GetCorrelationIdManager(IHttpContextAccessor httpContextAccessor)
        {
            Log.Debug("Getting ICorrelationIdManager from IHttContextAccessor");
            var correlationIdManager = httpContextAccessor?.HttpContext?.RequestServices?.GetService<ICorrelationIdManager>();

            return correlationIdManager;
        }
    }
}
