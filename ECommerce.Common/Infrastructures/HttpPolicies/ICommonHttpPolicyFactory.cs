using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.HttpPolicies
{
    public interface ICommonHttpPolicyFactory
    {
        /// <summary>
        /// Try to get a Policy from PolicyRegistry using the defined key from Func<HttpRequestMessage, string> keySelector
        /// If no Policy found, will create a new Policy from Func<string, IAsyncPolicy<HttpResponseMessage>> policyFactory then add it into PolicyRegistry
        /// </summary>
        /// <param name="request">The current HttpRequestMessage being called</param>
        /// <param name="policyFactory">Contains the logic of how a Policy should be created</param>
        /// <param name="keySelector">Contains the logic of how a key should be created</param>
        /// <returns></returns>
        IAsyncPolicy<HttpResponseMessage> GetPolicy(HttpRequestMessage request, Func<string, IAsyncPolicy<HttpResponseMessage>> policyFactory, Func<HttpRequestMessage, string> keySelector);

        /// <summary>
        /// Build a key from method (from HttpRequest) and route (from Endpoint) of the current HttpContext.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string BuildScopedEndpointPolicyKey(HttpRequestMessage request);
    }
}
