namespace ECommerce.Common.Infrastructures.Tracers
{
    public static class TracerConstants
    {
        /// <summary>
        /// This is a header key in HTTP requests that is used to pass the correlation ID.
        /// </summary>
        public const string CorrelationIdHeaderKey = "X-Correlation-Id";

        /// <summary>
        /// This is a new property name to describe the <see cref="CorrelationModel.TraceId"/> in the log context.
        /// </summary>
        public const string CorrelationIdLoggerPropertyName = "X-Correlation-Id";
    }
}
