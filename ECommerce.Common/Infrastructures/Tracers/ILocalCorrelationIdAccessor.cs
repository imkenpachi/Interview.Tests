namespace ECommerce.Common.Infrastructures.Tracers
{
    /// <summary>
    /// This interface is used to get or set the Correlation in an asynchronous control flow
    /// </summary>
    public interface ILocalCorrelationIdAccessor
    {
        /// <summary>
        /// Get or set the correlation model for the current request.
        /// </summary>
        CorrelationModel Correlation { get; set; }
    }
}