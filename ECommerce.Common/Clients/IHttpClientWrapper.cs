using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients
{
    /// <summary>
    /// Provides a wrapper interface for making HTTP requests with support for custom error handling, headers, authentication, and additional properties.
    /// </summary>
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Sends a GET request to the specified URI and returns the response stream.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with a response stream as result.</returns>
        Task<Stream> GetStreamAsync(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response stream, with asynchronous error conversion.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with a response stream as result.</returns>
        Task<Stream> GetStreamAsync(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a GET request to the specified URI and returns the HTTP response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a GET request to the specified URI and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<T> GetAsync<T>(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a GET request to the specified URI and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<T> GetAsync<T>(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request to the specified URI with the provided content and returns the HTTP response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request to the specified URI with the provided content and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PostAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request to the specified URI with the provided content and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PostAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request with a JSON body to the specified URI and returns the HTTP response.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request with a JSON body to the specified URI and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PostAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a POST request with a JSON body to the specified URI and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PostAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a DELETE request to the specified URI with the provided content and returns the HTTP response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> DeleteAsync(string uri, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a DELETE request to the specified URI and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> DeleteAsync<R>(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a DELETE request to the specified URI and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> DeleteAsync<R>(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request to the specified URI with the provided content and returns the HTTP response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request to the specified URI with the provided content and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PutAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request to the specified URI with the provided content and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PutAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request with a JSON body to the specified URI and returns the HTTP response.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request with a JSON body to the specified URI and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PutAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PUT request with a JSON body to the specified URI and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PutAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request to the specified URI with the provided content and returns the HTTP response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PatchAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request to the specified URI with the provided content and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PatchAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request to the specified URI with the provided content and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="requestContent">The HTTP content to send.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PatchAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request with a JSON body to the specified URI and returns the HTTP response.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<HttpResponseMessage> PatchAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request with a JSON body to the specified URI and returns the HTTP response, with custom error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegate">Delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PatchAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Sends a PATCH request with a JSON body to the specified URI and returns the HTTP response, with asynchronous error conversion.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <typeparam name="R">The type used for error conversion.</typeparam>
        /// <param name="uri">The request URI.</param>
        /// <param name="objectContent">The object to serialize as JSON.</param>
        /// <param name="errorResponseConversionDelegateAsync">Async delegate to convert error responses to exceptions.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="authToken">Optional authentication token.</param>
        /// <param name="ignoreNull">Whether to ignore null values in serialization.</param>
        /// <param name="properties">Optional additional properties.</param>
        /// <returns>A task representing the asynchronous operation, with the HTTP response as result.</returns>
        Task<R?> PatchAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = default, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null);
    }
}
