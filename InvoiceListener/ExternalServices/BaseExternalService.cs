namespace InvoiceListener.ExternalServices
{
    public abstract class BaseExternalService
    {
        public const string JSON_CONTENT = "application/json";
        protected virtual Func<HttpResponseMessage, Task<Exception>> HttpErrorsToServiceExceptionConverter()
        {
            static async Task<Exception> ConverterAsync(HttpResponseMessage errorResponseMessage)
            {
                var method = errorResponseMessage.RequestMessage?.Method;
                var errorStatusCode = errorResponseMessage.StatusCode;
                var endpoint = errorResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri;
                var errorContent = await errorResponseMessage.Content.ReadAsStringAsync();

                var errorMessage = $"Failed HTTP Method:{method} call to Endpoint:{endpoint} with ErrorStatusCode:{errorStatusCode}, ErrorContent:{errorContent}";

                return new HttpRequestException(errorMessage);
            }

            return ConverterAsync;
        }
    }
}
