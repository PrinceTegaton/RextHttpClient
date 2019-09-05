using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Rext
{
    public class RextHttpClient : IRextHttpClient, IDisposable
    {
        bool disposed = false;

        /// <summary>
        /// Rext configuration object
        /// </summary>
        public static RextConfigurationBundle ConfigurationBundle { get; set; } = new RextConfigurationBundle();

        /// <summary>
        /// Last http call statuscode
        /// </summary>
        public static int ReturnStatusCode { get; }

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public Stopwatch Stopwatch { get { return _stopwatch; } }
        private Stopwatch _stopwatch;


        public RextHttpClient(RextHttpCongifuration configuration = null)
        {
            if (configuration != null)
                ConfigurationBundle.HttpConfiguration = configuration;
        }

        public static void Setup(Action<RextConfigurationBundle> config)
        {
            config(ConfigurationBundle);
        }

        #region Interface Implementations

        public async Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            var data = await MakeRequest<T>(options, ContentType.Application_XML);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostXML<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload
            }, ContentType.Application_XML);

            return data;
        }

        public async Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            var data = await MakeRequest<T>(options, ContentType.Application_JSON);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload
            }, ContentType.Application_JSON);

            return data;
        }

        public async Task<CustomHttpResponse<string>> GetString(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            var data = await MakeRequest(options, ContentType.Text_Plain);
            return data;
        }

        public async Task<CustomHttpResponse<string>> GetString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload
            }, ContentType.Text_Plain);
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            var data = await MakeRequest<T>(options, ContentType.Application_XML);
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetXML<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload
            }, ContentType.Application_XML);

            return data;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            var data = await MakeRequest<T>(options, ContentType.Application_JSON);
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload
            }, ContentType.Application_JSON);

            return data;
        }

        public async Task<CustomHttpResponse<string>> MakeRequest(RextOptions options, string contentType)
        {
            var rsp = await ProcessRequest(options, contentType);
            rsp.Data = rsp.Content;
            rsp.Content = null;

            return rsp;
        }

        public async Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options, string contentType)
        {
            var rsp = await ProcessRequest(options, contentType);

            var newRsp = new CustomHttpResponse<T>
            {
                StatusCode = rsp.StatusCode,
                Message = rsp.Message
            };

            if (newRsp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (!string.IsNullOrEmpty(rsp.Content))
                {
                    bool throwExOnFail = options.ThrowExceptionOnDeserializationFailure ?? ConfigurationBundle.HttpConfiguration.ThrowExceptionOnDeserializationFailure;
                    (bool status, string message, T result) output = (false, null, default(T));
                    //T newObj = default(T);

                    if (contentType == ContentType.Application_JSON)
                    {
                        output = Helpers.DeserializeJSON<T>(rsp.Content, throwExOnFail);
                    }
                    else if (contentType == ContentType.Application_XML)
                    {
                        output = Helpers.DeserializeXML<T>(rsp.Content, throwExOnFail);
                    }

                    if (output.status)
                        newRsp.Data = output.result;
                    else
                        newRsp.Message = output.message;
                }
            }

            return newRsp;
        }

        #endregion


        //private async Task<CustomHttpResponse<string>> ProcessRequest(HttpMethod method, string url, object param = null, object header = null)
        private async Task<CustomHttpResponse<string>> ProcessRequest(RextOptions options, string contentType)
        {
            // execute all user actions pre-call
            ConfigurationBundle.BeforeCall();

            var rsp = new CustomHttpResponse<string>();
            var response = new HttpResponseMessage();
            string responseString = string.Empty;

            try
            {
                string queryString = string.Empty;

                // generate querystring from object if GET
                if (options.Method == HttpMethod.Get && options.Payload != null)
                    queryString = options.Payload.ToQueryString();

                var requestMsg = new HttpRequestMessage(options.Method, new Uri(options.Url + queryString));

                // set header if object has value
                if (this.Headers != null)
                    requestMsg.SetHeader(this.Headers);

                if (options.Header != null)
                    requestMsg.SetHeader(options.Header);

                if (!string.IsNullOrEmpty(contentType))
                    requestMsg.Headers.Add("Accept", contentType);

                // POST request
                if (options.Method == HttpMethod.Post & options.Payload != null)
                {
                    string strPayload = string.Empty;

                    // convert object to specified content-type
                    if (contentType == ContentType.Application_JSON)
                        strPayload = options.Payload.ToJson();
                    else if (contentType == ContentType.Application_XML)
                        strPayload = options.Payload.ToXml();
                    else
                        strPayload = options.Payload.ToString();

                    requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, contentType);
                }

                // use stopwatch to monitor httpcall duration
                if (ConfigurationBundle.EnableStopwatch)
                {
                    _stopwatch = new Stopwatch();
                    _stopwatch.Start();
                }

                response = await this.SendAsync(requestMsg, CancellationToken.None);

                // set watch value to public member
                if (ConfigurationBundle.EnableStopwatch) _stopwatch.Stop();

                rsp.StatusCode = response.StatusCode;
                responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    rsp.Content = responseString;
                    rsp.Message = "Http call successful";
                }
                else
                {
                    // this will always run before custom error-code actions
                    // always to ThrowExceptionIfNotSuccessResponse=false if you will use custom error-code actions
                    // perform checks for neccessary override
                    bool throwExIfNotSuccessRsp = options.ThrowExceptionIfNotSuccessResponse ?? ConfigurationBundle.HttpConfiguration.ThrowExceptionIfNotSuccessResponse;
                    if (throwExIfNotSuccessRsp)
                        throw new RextException($"Server response is {rsp.StatusCode}");

                    rsp.Content = responseString;
                    rsp.Message = "Http call completed but not successful";

                    // handle code specific error from user
                    int code = (int)response.StatusCode;
                    if (code > 0 && ConfigurationBundle.StatusCodesToHandle.Contains(code))
                        ConfigurationBundle.OnStatusCode(ReturnStatusCode);
                }

                // execute all user actions post-call
                ConfigurationBundle.AfterCall();
                return rsp;
            }
            catch (Exception ex)
            {
                // execute all user actions on error
                ConfigurationBundle.OnError();

                rsp.StatusCode = 0;

                if (ex?.Message.ToLower().Contains("a socket operation was attempted to an unreachable host") == true)
                    rsp.Message = "Internet connection error";
                else
                    rsp.Message = $"{ex?.Message} :: {ex?.InnerException?.InnerException?.Message ?? ex?.InnerException?.Message}";

                return rsp;
            }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            RextHttpCongifuration config = ConfigurationBundle.HttpConfiguration ?? new RextHttpCongifuration();

            // create httpclient from proxy handler
            using (var httpClientHandler = ProxyHttpClientHandler.ProxyHandler(config.ProxyAddress, config.RelaxSslCertValidation))
            {
                using (var client = ConfigurationBundle.HttpClient ?? new HttpClient(httpClientHandler))
                {
                    if (ConfigurationBundle.HttpConfiguration.Timeout > 0)
                        client.Timeout = TimeSpan.FromSeconds(ConfigurationBundle.HttpConfiguration.Timeout);
                    response = await client.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // cleanup objects
            }

            GC.SuppressFinalize(this);
        }
    }
}