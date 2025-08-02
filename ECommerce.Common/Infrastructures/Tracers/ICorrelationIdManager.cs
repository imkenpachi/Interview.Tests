using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.Tracers
{
    /// <summary>
    /// This component is used to manage correlation IDs across requests and services.
    /// </summary>
    /// <remarks>
    /// <para>- Should be registerred as scoped in IoC container.</para>
    /// <para>- In a scoped, the correlationId is only created once.</para>
    /// </remarks>
    public interface ICorrelationIdManager
    {
        /// <summary>
        /// Get scoped Correlation
        /// </summary>
        /// <returns>Returns a scoped Correlation</returns>
        CorrelationModel GetCorrelation();

        /// <summary>
        /// Get nested correlation depend on the <see cref="httpRequestHeaders"/>.
        /// </summary>
        /// <param name="httpRequestHeaders"></param>
        /// <returns>Returns scoped Correlation or generate new Correlation depend on <see cref="httpRequestHeaders"</returns>
        CorrelationModel GetNestedCorrelationDependOnHttpRequest(IHeaderDictionary httpRequestHeaders);

        /// <summary>
        /// Gets a nested correlation based on the <see cref="parentCorrelation">.
        /// </summary>
        /// <param name="parentCorrelation"></param>
        /// <returns>Returns scoped Correlation or generate new Correlation depend on <see cref="parentCorrelation"/></returns>
        CorrelationModel GetNestedCorrelation(CorrelationModel parentCorrelation);
    }
}
