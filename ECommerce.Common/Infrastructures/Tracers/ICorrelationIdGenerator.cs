using Microsoft.AspNetCore.Http;

namespace ECommerce.Common.Infrastructures.Tracers
{
    public interface ICorrelationIdGenerator
    {
        /// <summary>
        /// Generates a nested correlation model based on the HTTP request headers.
        /// </summary>
        /// <param name="httpRequestHeaders"></param>
        /// <returns></returns>
        CorrelationModel GetNestedCorrelation(IHeaderDictionary httpRequestHeaders);

        /// <summary>
        /// Generates a nested correlation model based on the parent correlation model.
        /// </summary>
        /// <param name="parentCorrelation"></param>
        /// <returns></returns>
        CorrelationModel GetNestedCorrelation(CorrelationModel parentCorrelation);

        /// <summary>
        /// Generates a new correlation model.
        /// </summary>
        /// <returns>A new <see cref="CorrelationModel"/></returns>
        CorrelationModel GenerateNewCorrelation();
    }
}