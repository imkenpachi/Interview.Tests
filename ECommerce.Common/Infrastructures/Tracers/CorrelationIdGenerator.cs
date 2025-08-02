using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Common.Infrastructures.Tracers
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        public CorrelationModel GenerateNewCorrelation()
        {
            return new CorrelationModel()
            {
                TraceId = Guid.NewGuid().ToString("N") // Generate a new trace ID as a GUID
            };
        }

        public CorrelationModel GetNestedCorrelation(IHeaderDictionary httpRequestHeaders)
        {
            var correlationIdRequestHeaderValue = string.Empty;

            if (httpRequestHeaders != null &&
                httpRequestHeaders.TryGetValue(TracerConstants.CorrelationIdHeaderKey, out var correlationIdHeaderValues) &&
                correlationIdHeaderValues.Count > 0)
            {
                correlationIdRequestHeaderValue = correlationIdHeaderValues.FirstOrDefault();
            }

            var correlationModel = string.IsNullOrEmpty(correlationIdRequestHeaderValue)
                ? GenerateNewCorrelation()
                : GenerateFromParentCorrelationId(correlationIdRequestHeaderValue);

            return correlationModel;
        }

        public CorrelationModel GetNestedCorrelation(CorrelationModel parentCorrelation)
        {
            return new CorrelationModel
            {
                TraceId = parentCorrelation.TraceId
            };
        }

        private static CorrelationModel GenerateFromParentCorrelationId(string correlationIdRequestHeaderValue)
        {
            return new CorrelationModel
            {
                TraceId = correlationIdRequestHeaderValue
            };
        }
    }
}
