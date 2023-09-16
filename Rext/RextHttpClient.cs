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
    ///<inheritdoc />
    /// <summary>
    /// Implementation of <see cref="IRextHttpClient"/>
    /// </summary>
    public class RextHttpClient : IRextHttpClient, IDisposable
    {
        #region LOCAL PROPS
        //bool disposed = false;

        /// <summary>
        /// Default HttpClient object
        /// </summary>
        private readonly HttpClient Client;

        /// <summary>
        /// Rext global configuration object
        /// </summary>
        public static RextConfigurationBundle ConfigurationBundle { get; set; } = new RextConfigurationBundle();

        /// <summary>
        /// Last http call statuscode
        /// </summary>
        public static int ReturnStatusCode { get; }

        /// <summary>
        /// List of headers appended from HeaderExtension
        /// </summary>
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        /// <summary>
        /// List of resiliency policies
        /// </summary>
        public List<ResiliencyPolicy> ResiliencyPolicies { get; set; } = new List<ResiliencyPolicy>();

        /// <summary>
        /// Get execution time of the http call when configured to run
        /// </summary>
        public Stopwatch Stopwatch { get; private set; }

        /// <summary>
        /// Set request timeout
        /// </summary>
        public TimeSpan? Timeout { get; set; }
        #endregion


        private readonly RextConfigurationBundle _localConfigurationBundle;


        /// <summary>
        /// Initialize a new instance of the <see cref="RextHttpClient"/> class with configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpClient"></param>
        public RextHttpClient(RextHttpCongifuration configuration = null, HttpClient httpClient = null)
        {
            // prevent global override from local instance configuration values
            _localConfigurationBundle = new RextConfigurationBundle
            {
                HttpClient = ConfigurationBundle.HttpClient,
                HttpConfiguration = configuration ?? ConfigurationBundle.HttpConfiguration ?? new RextHttpCongifuration(),
                EnableStopwatch = ConfigurationBundle.EnableStopwatch,
                BeforeCall = ConfigurationBundle.BeforeCall,
                AfterCall = ConfigurationBundle.AfterCall,
                MergeResiliencyPoliciesAddedFromExtension = ConfigurationBundle.MergeResiliencyPoliciesAddedFromExtension,
                OnStatusCode = ConfigurationBundle.OnStatusCode,
                StatusCodesToHandle = ConfigurationBundle.StatusCodesToHandle,
                ResiliencyPolicies = ConfigurationBundle.ResiliencyPolicies,
                OnError = ConfigurationBundle.OnError,
                SuppressRextExceptions = ConfigurationBundle.SuppressRextExceptions
            };

            // create httpclient from proxy handler
            HttpClientHandler httpClientHandler = CustomHttpClientHandler.CreateHandler(
                    _localConfigurationBundle.HttpConfiguration.ProxyAddress,
                    _localConfigurationBundle.HttpConfiguration.RelaxSslCertValidation,
                    _localConfigurationBundle.HttpConfiguration.Certificate);

            this.Client ??= httpClient ?? _localConfigurationBundle.HttpClient ?? new HttpClient(httpClientHandler);

            if (_localConfigurationBundle.HttpConfiguration.Timeout > 0)
            {
                this.Client.Timeout = TimeSpan.FromSeconds(_localConfigurationBundle.HttpConfiguration.Timeout);
            }

            // override from extension
            if (this.Timeout.HasValue)
            {
                this.Client.Timeout = this.Timeout.Value;
            }
        }

        /// <summary>
        /// Global setup for Rext behaviors and actions using <see cref="RextConfigurationBundle"/> object
        /// </summary>
        /// <param name="config"></param>
        public static void Setup(Action<RextConfigurationBundle> config)
        {
            config(ConfigurationBundle);
        }

        #region Interface Implementations

        #region POST

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PostForm<T>(RextOptions options, bool isUrlEncoded = false)
        {
            options.Method = HttpMethod.Post;
            options.IsForm = true;
            options.ExpectedResponseFormat = ContentType.Application_JSON;
            options.IsUrlEncoded = isUrlEncoded;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
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

        /// <summary>
        /// Post XML content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_XML;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Post XML content for XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
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

        /// <summary>
        /// Post XML content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
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

        /// <summary>
        /// Post XML content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PostXMLForString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_XML;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest(options);
            return data;
        }

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
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

        /// <summary>
        /// Post JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PostJSONForString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Text_Plain
            });

            return data;
        }

        /// <summary>
        /// Post JSON content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PostJSONForString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Application_JSON;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }

        /// <summary>
        /// Post plain string content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Plain string response</returns>
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

        /// <summary>
        /// Post plain string content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Plain string response</returns>
        public async Task<CustomHttpResponse<string>> PostString(RextOptions options)
        {
            options.Method = HttpMethod.Post;
            options.ContentType = ContentType.Text_Plain;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }

        #endregion

        #region GET

        /// <summary>
        /// Get plain string result. Accepts advanced options
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>String response</returns>
        public async Task<CustomHttpResponse<string>> GetString(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }

        /// <summary>
        /// get plain string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>String response</returns>
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

        /// <summary>
        /// Get XML result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Application_XML;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Get XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
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

        /// <summary>
        /// Get JSON result deserialized to custom type. Accepts advanced options.
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Get;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Get JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
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
        #endregion

        #region DELETE

        /// <summary>
        /// Delete for string result deserialized to custom type
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send will be serialized to querystring</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> Delete(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Delete,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Text_Plain,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        /// <summary>
        /// Delete for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call. Payload will be serialized to querystring</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> DeleteJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Delete;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Delete for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send will be serialized to querystring</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> DeleteJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Delete,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }
        #endregion

        #region PUT
        /// <summary>
        /// Put JSON content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PutJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Put;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Put JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PutJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Put,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        /// <summary>
        /// Put JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PutJSONForString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Put,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Text_Plain
            });

            return data;
        }

        /// <summary>
        /// Put JSON content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PutJSONForString(RextOptions options)
        {
            options.Method = HttpMethod.Put;
            options.ContentType = ContentType.Application_JSON;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }
        #endregion

        #region PATCH
        /// <summary>
        /// Patch JSON content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PatchJSON<T>(RextOptions options)
        {
            options.Method = HttpMethod.Patch;
            options.ExpectedResponseFormat = ContentType.Application_JSON;

            var data = await MakeRequest<T>(options);
            return data;
        }

        /// <summary>
        /// Patch JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> PatchJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Patch,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Application_JSON
            });

            return data;
        }

        /// <summary>
        /// Patch JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PatchJSONForString(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest(new RextOptions
            {
                Url = url,
                Method = HttpMethod.Patch,
                Header = header,
                Payload = payload,
                ContentType = ContentType.Application_JSON,
                ExpectedResponseFormat = ContentType.Text_Plain
            });

            return data;
        }

        /// <summary>
        /// Patch JSON content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> PatchJSONForString(RextOptions options)
        {
            options.Method = HttpMethod.Put;
            options.ContentType = ContentType.Application_JSON;
            options.ExpectedResponseFormat = ContentType.Text_Plain;

            var data = await MakeRequest(options);
            return data;
        }
        #endregion


        /// <summary>
        /// Process direct request with plain string response. This method is called by all verb actions
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<string>> MakeRequest(RextOptions options)
        {
            var rsp = await ProcessRequest(options);
            rsp.Data = rsp.Content;
            rsp.Content = null;

            return rsp;
        }

        /// <summary>
        /// Process direct request with deserialized generic response. This method is called by all verb actions
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        public async Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options)
        {
            var rsp = await ProcessRequest(options);

            var newRsp = new CustomHttpResponse<T>
            {
                StatusCode = rsp.StatusCode,
                Message = rsp.Message,
                Content = rsp.Content
            };

            bool deserializeSuccessOnly = options?.DeserializeSuccessResponseOnly ?? _localConfigurationBundle.HttpConfiguration.DeserializeSuccessResponseOnly;

            if (!string.IsNullOrEmpty(rsp.Content) && (newRsp.StatusCode == System.Net.HttpStatusCode.OK || !deserializeSuccessOnly))
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
                    if (newRsp.StatusCode != System.Net.HttpStatusCode.OK && !deserializeSuccessOnly)
                {
                    newRsp.Message = $"Type deserialization failed: To prevent deserialization of unsuccessful response types, set DeserializeSuccessResponseOnly=true";

                    if (throwExOnFail)
                        throw new RextException(newRsp.Message + " --> To prevent deserialization of unsuccessful response types, set DeserializeSuccessResponseOnly = true");

                    return newRsp;
                }
                else
                {
                    newRsp.Message = output.message;
                }
            }
            else
                newRsp.Message += " --> To allow deserialization even when response status code is not successful, set DeserializeSuccessResponseOnly = false";


            return newRsp;
        }

        #endregion


        private async Task<CustomHttpResponse<string>> ProcessRequest(RextOptions options)
        {
            ResiliencyPolicy policy = null;
            int retryCount = 0;
        start:
            if (this.Client == null)
                throw new ArgumentNullException("HttpClient object cannot be null");

            // validate essential params
            if (string.IsNullOrEmpty(options.Url))
            {
                if (string.IsNullOrEmpty(_localConfigurationBundle.HttpConfiguration.BaseUrl))
                    throw new ArgumentNullException(nameof(options.Url), $"{nameof(options.Url)} is required. Provide fully qualified api endpoint.");
                else
                    throw new ArgumentNullException(nameof(options.Url), $"{nameof(options.Url)} is required. Provide the other part of api endpoint to match the BaseUrl.");
            }

            if (options.Method == null)
                throw new ArgumentNullException(nameof(options.Method), $"{nameof(options.Method)} is required. Use GET, POST etc..");

            if (string.IsNullOrEmpty(options.ContentType))
                throw new ArgumentNullException(nameof(options.ContentType), $"{nameof(options.ContentType)} is required. Use application/json, text.plain etc..");

            if (string.IsNullOrEmpty(options.ExpectedResponseFormat))
                throw new ArgumentNullException(nameof(options.ExpectedResponseFormat), $"{nameof(options.ExpectedResponseFormat)} is required. Use application/json, text.plain etc..");


            // execute all user actions pre-call
            _localConfigurationBundle.BeforeCall?.Invoke();

            var rsp = new CustomHttpResponse<string>();
            var response = new HttpResponseMessage();
            string responseString = string.Empty;

            try
            {
                Uri uri = options.CreateUri(_localConfigurationBundle.HttpConfiguration.BaseUrl);
                if (uri == null)
                    throw new UriFormatException("Invalid request Uri");

                var requestMsg = new HttpRequestMessage(options.Method, uri);

                // set header if object has value
                if (this.Headers != null)
                    requestMsg.SetHeader(this.Headers);

                if (options.Header != null)
                    requestMsg.SetHeader(options.Header);

                if (_localConfigurationBundle.HttpConfiguration.Header != null)
                   requestMsg.SetHeader(_localConfigurationBundle.HttpConfiguration.Header);
                
                if (!string.IsNullOrEmpty(options.ContentType))
                    requestMsg.SetHeader("Accept", options.ExpectedResponseFormat);

                // POST request
                if (options.Method != HttpMethod.Get && options.Payload != null)
                {
                    string strPayload = string.Empty;

                    if (options.IsForm && !options.IsUrlEncoded)
                    {
                        strPayload = options.Payload.ToQueryString()?.TrimStart('?');

                        // form-data content post
                        if (options.IsUrlEncoded)
                            // handle application/x-www-form-urlencoded
                            requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, "application/x-www-form-urlencoded");
                        
                        // handle multipart/form-data
                        var formData = strPayload.Split('&');
                        var mpfDataBucket = new MultipartFormDataContent();

                        foreach (var i in formData)
                        {
                            var row = i.Split('=');
                            if (row.Length == 2) // check index to avoid null
                            {
                                mpfDataBucket.Add(new StringContent(row[1]), row[0]);
                            }
                        }

                        requestMsg.Content = mpfDataBucket;
                    }
                    else
                    {
                        // convert object to specified content-type
                        if (options.ContentType == ContentType.Application_JSON)
                        {
                            strPayload = options.Payload.ToJson(_localConfigurationBundle.HttpConfiguration?.JsonSerializerSettings);
                        }
                        else if (options.ContentType == ContentType.Application_XML)
                        {
                            strPayload = options.Payload.ToXml(_localConfigurationBundle.HttpConfiguration?.DefaultXmlEncoding);
                        }
                        else
                        {
                            strPayload = options.Payload.ToString();
                        }

                        requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, options.ContentType);
                    }
                }

                // use stopwatch to monitor httpcall duration
                if (_localConfigurationBundle.EnableStopwatch)
                {
                    Stopwatch = new Stopwatch();
                    Stopwatch.Start();
                }

                // check if HttpCompletionOption option is used
                HttpCompletionOption httpCompletionOption = _localConfigurationBundle.HttpConfiguration.HttpCompletionOption;
                if (options.HttpCompletionOption.HasValue && _localConfigurationBundle.HttpConfiguration.HttpCompletionOption != options.HttpCompletionOption.Value)
                    httpCompletionOption = options.HttpCompletionOption.Value;

                response = await this.Client.SendAsync(requestMsg, httpCompletionOption, CancellationToken.None).ConfigureAwait(false);

                // set watch value to public member
                if (_localConfigurationBundle.EnableStopwatch)
                {
                    Stopwatch.Stop();
                }


                #region handle resiliency policies
                List<ResiliencyPolicy> resiliencyPolicies = null;
                if (_localConfigurationBundle.MergeResiliencyPoliciesAddedFromExtension)
                {
                    // merge global and local policies
                    if (this.ResiliencyPolicies != null && this.ResiliencyPolicies.Any())
                    {
                        resiliencyPolicies = _localConfigurationBundle.ResiliencyPolicies;
                        resiliencyPolicies.AddRange(ResiliencyPolicies);
                    }
                }
                else
                {
                    // override global policies
                    resiliencyPolicies = ResiliencyPolicies ?? _localConfigurationBundle.ResiliencyPolicies;
                }

                if (policy != null || (resiliencyPolicies != null && resiliencyPolicies.Any()))
                {
                    policy ??= _localConfigurationBundle.ResiliencyPolicies.FirstOrDefault(a => (a.StatusCodes.Contains((int)response.StatusCode) || a.StatusCode == (int)response.StatusCode) &&
                    (options?.IgnoreStatusCodeInResiliencyPolicies != null &&
                    !options.IgnoreStatusCodeInResiliencyPolicies.Contains((int)response.StatusCode)));

                    if (policy != null && retryCount < policy.Retry)
                    {
                        if (policy.Interval.HasValue)
                        {
                            await Task.Delay(policy.Interval.Value);
                        }

                        retryCount++;
                        goto start;
                    }

                }
                #endregion

                rsp.StatusCode = response.StatusCode;
                rsp.Retries = retryCount;

                if (options.IsStreamResponse)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    if (stream.Length > 0)
                    {
                        using var rd = new StreamReader(stream);
                        responseString = rd.ReadToEnd();
                    }
                }
                else
                {
                    responseString = await response.Content.ReadAsStringAsync();
                }

                if (response.IsSuccessStatusCode)
                {
                    rsp.Content = responseString;
                    rsp.Message = "Http call successful";
                }
                else
                {
                    // this will always run before custom error-code actions
                    // always set ThrowExceptionIfNotSuccessResponse=false if you will use custom error-code actions
                    // perform checks for neccessary override
                    bool throwExIfNotSuccessRsp = options.ThrowExceptionIfNotSuccessResponse ?? _localConfigurationBundle.HttpConfiguration.ThrowExceptionIfNotSuccessResponse;
                    if (throwExIfNotSuccessRsp)
                        throw new RextException($"Server response is {rsp.StatusCode}");


                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        rsp.Content = $"Url not found: {requestMsg.RequestUri}";
                    else
                        rsp.Content = responseString;

                    rsp.Message = "Http call completed but not successful";

                    // handle code specific error from user
                    int code = (int)response.StatusCode;
                    if (code > 0 && _localConfigurationBundle.StatusCodesToHandle != null && _localConfigurationBundle.StatusCodesToHandle.Contains(code))
                        _localConfigurationBundle.OnStatusCode?.Invoke(ReturnStatusCode);
                }

                // execute all user actions post-call
                _localConfigurationBundle.AfterCall?.Invoke(uri.ToString(), rsp);
                return rsp;
            }
            catch (Exception ex)
            {
                // execute all user actions on error
                _localConfigurationBundle.OnError?.Invoke();

                if (ex is UriFormatException)
                    throw;

                if (!ConfigurationBundle.SuppressRextExceptions)
                    throw ex;

                if (ex?.Message.ToLower().Contains("a socket operation was attempted to an unreachable host") == true)
                    rsp.Message = "Network connection error";
                else if (ex?.Message.ToLower().Contains("the character set provided in contenttype is invalid") == true)
                    rsp.Message = "Invald response ContentType. If you are expecting a Stream response then set RextOptions.IsStreamResponse=true";
                else
                    rsp.Message = ex?.Message; //{ex?.InnerException?.Message ?? ex?.InnerException?.Message}";

                return rsp;
            }
        }

        private bool _disposed = false;

        /// <summary>
        /// Dispose current instance of RextHttpClient
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Client.Dispose();
                    this.Stopwatch = null;
                }

                _disposed = true;
            }
        }
    }
}