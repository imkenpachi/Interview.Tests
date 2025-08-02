using System.Net;

namespace ECommerce.Common.Exceptions
{
    public abstract class CommonApplicationException(HttpStatusCode statusCode, string errorCode, string errorMessage) : Exception(errorMessage)
    {
        public string ErrorCode { get; } = errorCode;
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
