using ECommerce.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.Tracers.HttpMessageHandlers
{
    public class CorrelationIdMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalCorrelationIdAccessor _localCorrelationIdAccessor;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public CorrelationIdMessageHandler(IHttpContextAccessor httpContextAccessor, ILocalCorrelationIdAccessor localCorrelationIdAccessor, ICorrelationIdGenerator correlationIdGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _localCorrelationIdAccessor = localCorrelationIdAccessor;
            _correlationIdGenerator = correlationIdGenerator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains(TracerConstants.CorrelationIdHeaderKey))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var correlationModel = CorrelationIdHelpers.GetCorrelation(
                _localCorrelationIdAccessor,
                _httpContextAccessor,
                _correlationIdGenerator
                );

            var correlationId = correlationModel.GetCorretionId();

            Log.Debug("{HttpMessageHandlerName} assigned correlationId: {CorrelationId} to request {Method} {AbsoluteUri}.",
                nameof(CorrelationIdMessageHandler),
                correlationId,
                request.Method.Method,
                request.RequestUri?.AbsoluteUri
                );

            request.Headers.Add(TracerConstants.CorrelationIdHeaderKey, correlationId);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
