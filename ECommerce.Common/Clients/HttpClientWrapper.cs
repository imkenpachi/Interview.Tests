using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using ECommerce.Common.Helpers;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients
{
    public sealed class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private const string JSON_CONTENT = "application/json";
        private const string BEARER = "Bearer";

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stream> GetStreamAsync(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authToken, headers: headers, properties: properties);

            var response = await SendAsync(request, cancellationToken ?? CancellationToken.None, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }

        public async Task<Stream> GetStreamAsync(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authToken, headers: headers, properties: properties);

            var response = await SendAsync(request, cancellationToken ?? CancellationToken.None, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }

        public async Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Get, uri, authToken, headers: headers, properties: properties);
            return await SendAsync(request, cancellationToken ?? CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await GetAsync(uri, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }

        public async Task<T> GetAsync<T>(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await GetAsync(uri, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Post, uri, authToken, content: requestContent, headers: headers, properties: properties);
            return await SendAsync(request, cancellationToken ?? CancellationToken.None);
        }

        public async Task<R?> PostAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate
        , IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PostAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }


        public async Task<R?> PostAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync
        , IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PostAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }
        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null,
            string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PostAsync(uri, content, headers, cancellationToken, authToken, properties);
        }

        public async Task<R?> PostAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken?
        cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PostAsync<R>(uri, content, errorResponseConversionDelegate, headers, cancellationToken, authToken, properties);
        }

        public async Task<R?> PostAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PostAsync<R>(uri, content, errorResponseConversionDelegateAsync, headers, cancellationToken, authToken, properties);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null
            , IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Delete, uri, authToken, headers: headers, properties: properties);
            return await SendAsync(request, cancellationToken ?? CancellationToken.None);
        }

        public async Task<R?> DeleteAsync<R>(string uri, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null,
            string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await DeleteAsync(uri, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }

        public async Task<R?> DeleteAsync<R>(string uri, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null,
        string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await DeleteAsync(uri, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }

        public async Task<HttpResponseMessage> PutAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null,
IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Put, uri, authToken, content: requestContent, headers: headers, properties: properties);
            return await SendAsync(request, cancellationToken ?? CancellationToken.None);
        }

        public async Task<R?> PutAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate
        , IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PutAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }

        public async Task<R?> PutAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync,
            IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PutAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null,
        CancellationToken? cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            var response = await PutAsync(uri, content, headers, cancellationToken, authToken, properties);
            return response;
        }

        public async Task<R> PutAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null,
        CancellationToken? cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PutAsync<R>(uri, content, errorResponseConversionDelegate, headers, cancellationToken, authToken, properties);
        }

        public async Task<R> PutAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null,
        CancellationToken? cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PutAsync<R>(uri, content, errorResponseConversionDelegateAsync, headers, cancellationToken, authToken, properties);
        }

        public async Task<HttpResponseMessage> PatchAsync(string uri, HttpContent requestContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null,
IDictionary<string, object>? properties = null)
        {
            var request = CreateRequest(HttpMethod.Patch, uri, authToken, content: requestContent, headers: headers, properties: properties);
            return await SendAsync(request, cancellationToken ?? CancellationToken.None);
        }
        public async Task<R?> PatchAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate,
            IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PatchAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = errorResponseConversionDelegate(response);
            throw exception;
        }

        public async Task<R?> PatchAsync<R>(string uri, HttpContent requestContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync,
            IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null, string? authToken = null, IDictionary<string, object>? properties = null)
        {
            var response = await PatchAsync(uri, requestContent, headers, cancellationToken, authToken, properties);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<R>(await response.Content.ReadAsStringAsync());
            }
            var exception = await errorResponseConversionDelegateAsync(response);
            throw exception;
        }

        public async Task<HttpResponseMessage> PatchAsJsonAsync<T>(string uri, T objectContent, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null,
            string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PatchAsync(uri, content, headers, cancellationToken, authToken, properties);
        }

        public async Task<R?> PatchAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Exception> errorResponseConversionDelegate, IDictionary<string, string>? headers = null,
            CancellationToken? cancellationToken = null, string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PatchAsync<R>(uri, content, errorResponseConversionDelegate, headers, cancellationToken, authToken, properties);
        }

        public async Task<R?> PatchAsJsonAsync<T, R>(string uri, T objectContent, Func<HttpResponseMessage, Task<Exception>> errorResponseConversionDelegateAsync, IDictionary<string, string>? headers = null, CancellationToken? cancellationToken = null,
        string? authToken = null, bool ignoreNull = true, IDictionary<string, object>? properties = null)
        {
            var content = new StringContent(Serializer.SerializeObject(objectContent, ignoreNull), Encoding.UTF8, JSON_CONTENT);
            return await PatchAsync<R>(uri, content, errorResponseConversionDelegateAsync, headers, cancellationToken, authToken, properties);
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            try
            {
                Log.Information("[Request] {Method} {AbsoluteUri} content payload: {ContentLength} bytes.",
                    requestMessage.Method,
                    requestMessage.RequestUri?.AbsoluteUri,
                    requestMessage.Content?.Headers?.ContentLength
                );

                var timer = Stopwatch.StartNew();
                var response = cancellationToken == CancellationToken.None ?
                    await _httpClient.SendAsync(requestMessage, completionOption) :
                    await _httpClient.SendAsync(requestMessage, completionOption, cancellationToken);
                timer.Stop();

                var isError = !response.IsSuccessStatusCode;

                Log.Write(isError ? LogEventLevel.Error : LogEventLevel.Information,
                    "[Response] {Method} {AbsoluteUri} in duration {Duration}ms with status code {StatusCode} and response payload: {ContentLength} bytes.",
                    response?.RequestMessage?.Method,
                    response?.RequestMessage?.RequestUri?.AbsoluteUri,
                    timer.ElapsedMilliseconds,
                    response?.StatusCode,
                    response?.Content?.Headers?.ContentLength);

                return response;
            }
            // This is to make sure this type of exception is logged correctly beause upstreams could be
            // confused with others and loged with not so descriptive message if they did not catch this specific type of exception.
            catch (BrokenCircuitException ex)
            {
                Log.Warning(ex, "[CircuitBreaker] opened for {Method} {RequestUri}.", requestMessage.Method.Method, requestMessage.RequestUri?.AbsoluteUri);
                throw;
            }
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, string? authToken = null, HttpContent? content = null,
            IDictionary<string, string>? headers = null, IDictionary<string, object>? properties = null)
        {
            var request = new HttpRequestMessage(method, uri);

            if (authToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue(BEARER, authToken);

            if (content != null)
            {
                request.Content = content;
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    request.Options.Set(new HttpRequestOptionsKey<object>(property.Key), property.Value);
                }
            }
            return request;
        }
    }
}
