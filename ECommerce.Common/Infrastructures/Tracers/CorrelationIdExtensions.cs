using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.Tracers
{
    public static class  CorrelationIdExtensions
    {
        /// <summary>
        /// This extension method enriches the log context with the correlation ID from the provided <see cref="CorrelationModel"/>.
        /// </summary>
        /// <param name="correlationModel"></param>
        /// <returns></returns>
        public static IDisposable EnrichLogContext(this CorrelationModel correlationModel)
        {
            return LogContext.PushProperty(TracerConstants.CorrelationIdLoggerPropertyName, correlationModel.TraceId);
        }
    }
}
