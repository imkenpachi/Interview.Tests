using ECommerce.Common.Exceptions;

namespace ECommerce.Common.Exceptions
{
    public class ExternalServiceRequestFailedException : CommonApplicationException
    {
        public ExternalServiceRequestFailedException() : base(System.Net.HttpStatusCode.InternalServerError, "external_service_call_failed", "External service call failed")
        {

        }
    }
}
