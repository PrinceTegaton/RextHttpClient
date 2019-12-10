using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rext
{
    /// <summary>
    /// Rext HttpClient wrapper implementation
    /// </summary>
    public class RextHttpClient : IRextHttpClient
    {
        //bool disposed = false;

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

        /// <summary>
        /// Initialize class
        /// </summary>
        /// <param name="configuration"></param>
        public RextHttpClient(RextHttpCongifuration configuration = null)
        {
            if (configuration != null)
                ConfigurationBundle.HttpConfiguration = configuration;
        }

        /// <summary>
        /// Global setup for Rext behaviors and actions
        /// </summary>
        /// <param name="config"></param>
        public static void Setup(Action<RextConfigurationBundle> config)
        {
            config(ConfigurationBundle);
        }

        #region Interface Implementations
        
        public async Task<CustomHttpResponse<T>> PostForm<T>(RextOptions options, bool isUrlEncoded = false)
        {
            options.Method = HttpMethod.Post;
            options.IsForm = true;
            options.ExpectedResponseFormat = ContentType.Application_JSON;
            options.IsUrlEncoded = isUrlEncoded;

            var data = await MakeRequest<T>(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostForm<T>(string url, object payload = null, object header = null, bool isUrlEncoded = false)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON,
                IsForm = true,
                IsUrlEncoded = isUrlEncoded
            });

            return data;
        }

        public async Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_XML;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest<T>(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostXML<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_XML,
                ExpectedResponseFormat = ContentType.Application_XML
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> PostXMLForString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_XML,
                ExpectedResponseFormat = ContentType.Application_XML
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> PostXMLForString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_XML;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> PostJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> PostJSONForString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> PostJSONForString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_JSON;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest(options);
            return data;
        }

        public async Task<CustomHttpResponse<string>> PostString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Text_Plain,
                ExpectedResponseFormat = ContentType.Text_Plain
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> PostString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Text_Plain;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }

        public async Task<CustomHttpResponse<string>> GetString(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }

        public async Task<CustomHttpResponse<string>> GetString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Text_Plain,
                ExpectedResponseFormat = ContentType.Text_Plain
            });
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest<T>(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetXML<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_XML,
                ExpectedResponseFormat = ContentType.Application_XML
            });

            return data;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        public async Task<CustomHttpResponse<string>> MakeRequest(RextOptions options)
        {
            var rsp = await ProcessRequest(options);
            rsp.Data = rsp.Content;
            rsp.Content = null;

            return rsp;
        }

        public async Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options)
        {
            var rsp = await ProcessRequest(options);

            var newRsp = new CustomHttpResponse<T>
            {
                StatusCode = rsp.StatusCode,
                Message = rsp.Message,
                Content = rsp.Content
            };

            if (newRsp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (!string.IsNullOrEmpty(rsp.Content))
                {
                    bool throwExOnFail = options.ThrowExceptionOnDeserializationFailure ?? ConfigurationBundle.HttpConfiguration.ThrowExceptionOnDeserializationFailure;
                    (bool status, string message, T result) output = (false, null, default(T));

                    if (options.ExpectedResponseFormat == ContentType.Application_JSON)
                    {
                        output = Helpers.DeserializeJSON<T>(rsp.Content, throwExOnFail);
                    }
                    else if (options.ExpectedResponseFormat == ContentType.Application_XML)
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


        private async Task<CustomHttpResponse<string>> ProcessRequest(RextOptions options)
        {
            // validate essential params
            if (string.IsNullOrEmpty(options.Url))
            {
                if (string.IsNullOrEmpty(ConfigurationBundle.HttpConfiguration.BaseUrl))
                    throw new ArgumentNullException(nameof(options.Url), $"{nameof(options.Url)} is required. Provide fully qualified api endpoint.");
                else
                    throw new ArgumentNullException(nameof(options.Url), $"{nameof(options.Url)} is required. Provide the other part of api endpoint to match the BaseUrl.");
            }

            if (options.Method == null)
                throw new ArgumentNullException(nameof(options.Method), $"{nameof(options.Method)} is required. Use GET, POST etc..");

            if (string.IsNullOrEmpty(options.ContentType))
                throw new ArgumentNullException(nameof(options.ContentType), $"{nameof(options.ContentType) } is required. Use application/json, text.plain etc..");

            if (string.IsNullOrEmpty(options.ExpectedResponseFormat))
                throw new ArgumentNullException(nameof(options.ExpectedResponseFormat), $"{nameof(options.ExpectedResponseFormat)} is required. Use application/json, text.plain etc..");


            // execute all user actions pre-call
            ConfigurationBundle.BeforeCall?.Invoke();

            var rsp = new CustomHttpResponse<string>();
            var response = new HttpResponseMessage();
            string responseString = string.Empty;

            try
            {
                Uri uri = options.CreateUri(ConfigurationBundle.HttpConfiguration.BaseUrl);
                if (uri == null)
                    throw new UriFormatException("Invalid request Uri");


                var requestMsg = new HttpRequestMessage(options.Method, uri);

                // set header if object has value
                if (this.Headers != null)
                    requestMsg.SetHeader(this.Headers);

                if (options.Header != null)
                    requestMsg.SetHeader(options.Header);

                if (!string.IsNullOrEmpty(options.ContentType))
                    requestMsg.SetHeader("Accept", options.ExpectedResponseFormat);

                // POST request
                if (options.Method == HttpMethod.Post & options.Payload != null)
                {
                    string strPayload = string.Empty;

                    if (options.IsForm)
                    {
                        strPayload = options.Payload.ToQueryString()?.TrimStart('?');

                        // form-data content post
                        if (!options.IsUrlEncoded)
                        {
                            // handle multipart/form-data
                            var formData = strPayload.Split('&');
                            var mpfDataBucket = new MultipartFormDataContent();

                            foreach (var i in formData)
                            {
                                var row = i.Split('=');
                                if (row.Length == 2) // check index to avoid null
                                    mpfDataBucket.Add(new StringContent(row[1]), row[0]);
                            }

                            requestMsg.Content = mpfDataBucket;
                        }
                        else
                        {
                            // handle application/x-www-form-urlencoded
                            requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, "application/x-www-form-urlencoded");
                        }
                    }
                    else
                    {
                        // convert object to specified content-type
                        if (options.ContentType == ContentType.Application_JSON)
                            strPayload = options.Payload.ToJson();
                        else if (options.ContentType == ContentType.Application_XML)
                            strPayload = options.Payload.ToXml();
                        else
                            strPayload = options.Payload.ToString();

                        requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, options.ContentType);
                    }
                }

                // use stopwatch to monitor httpcall duration
                if (ConfigurationBundle.EnableStopwatch)
                {
                    _stopwatch = new Stopwatch();
                    _stopwatch.Start();
                }

                response = await this.Send(requestMsg, CancellationToken.None, options.IsForm);

                // set watch value to public member
                if (ConfigurationBundle.EnableStopwatch) _stopwatch.Stop();

                rsp.StatusCode = response.StatusCode;

                if (options.IsStreamResponse)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    if (stream.Length > 0)
                    {
                        using (var rd = new StreamReader(stream))
                            responseString = rd.ReadToEnd();
                    }
                }
                else
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
                        ConfigurationBundle.OnStatusCode?.Invoke(ReturnStatusCode);
                }

                // execute all user actions post-call
                ConfigurationBundle.AfterCall?.Invoke();
                return rsp;
            }
            catch (Exception ex)
            {
                // execute all user actions on error
                ConfigurationBundle.OnError?.Invoke();

                if (ConfigurationBundle.SuppressRextExceptions)
                {
                    if (ex?.Message.ToLower().Contains("a socket operation was attempted to an unreachable host") == true)
                        rsp.Message = "Internet connection error";
                    else if (ex?.Message.ToLower().Contains("the character set provided in contenttype is invalid") == true)
                        rsp.Message = "Invald response ContentType. If you are expecting a Stream response then set RextOptions.IsStreamResponse=true";
                    else
                        rsp.Message = ex?.Message; //{ex?.InnerException?.Message ?? ex?.InnerException?.Message}";

                    return rsp;
                }
                else
                {
                    throw ex;
                }
            }
        }

        private async Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken cancellationToken, bool isForm = false)
        {
            HttpResponseMessage response = null;
            RextHttpCongifuration config = ConfigurationBundle.HttpConfiguration ?? new RextHttpCongifuration();

            // create httpclient from proxy handler
            using (var httpClientHandler = ProxyHttpClientHandler.ProxyHandler(config.ProxyAddress, config.RelaxSslCertValidation, config.Certificate))
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


        //public void Dispose()
        //{
        //    Dispose(true);
        //}

        //protected void Dispose(bool disposing)
        //{
        //    if (disposed) return;

        //    if (disposing)
        //    {
        //        // cleanup objects
        //    }

        //    GC.SuppressFinalize(this);
        //}
    }
}