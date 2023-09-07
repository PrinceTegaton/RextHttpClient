using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Rext
{
    /// <summary>
    /// Certificate model class
    /// </summary>
    public class CertificateInfo
    {
        /// <summary>
        /// File path to certificate on machine. This is used ahead of CertificateBytes if a value is provided for both
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Certificate password if any
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Certificate byte content
        /// </summary>
        public byte[] CertificateBytes { get; set; }
    }


    /// <summary>
    /// RextConfigurationBundle class
    /// </summary>
    public class RextConfigurationBundle
    {
        /// <summary>
        /// Configure httpclient and actions
        /// </summary>
        public RextHttpCongifuration HttpConfiguration { get; set; } = new RextHttpCongifuration();

        /// <summary>
        /// Create a custom client for usage. This will discard every setting in RextHttpCongifuration.HttpConfiguration
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// This allow you to retrieve exception messages in RextHttpClient.Message. Set to false if you want to handle all exceptions from your code
        /// </summary>
        public bool SuppressRextExceptions { get; set; } = true;

        /// <summary>
        /// Execute action before any http call
        /// </summary>
        public Action BeforeCall { get; set; }

        /// <summary>
        /// Execute action after any http call
        /// </summary>
        /// <remarks>Url and Response are available</remarks>
        public Action<string, CustomHttpResponse> AfterCall { get; set; }

        /// <summary>
        /// Execute action when an exception is thrown by RextHttpClient
        /// </summary>
        public Action OnError { get; set; }

        /// <summary>
        /// Execute action for a specific statuscode
        /// </summary>
        public Action<int> OnStatusCode { get; set; }

        /// <summary>
        /// Array for all statuscodes to run custom action for
        /// </summary>
        public int[] StatusCodesToHandle { get; set; }

        /// <summary>
        /// Determine if Rext Stopwatch should be used. Value is True by default
        /// </summary>
        public bool EnableStopwatch { get; set; } = true;

        /// <summary>
        /// Configure resiliency policies
        /// </summary>
        public List<ResiliencyPolicy> ResiliencyPolicies { get; set; }

        /// <summary>
        /// If true, policies added via extension will be merged with global configuration
        /// </summary>
        public bool MergeResiliencyPoliciesAddedFromExtension { get; set; } = true;
    }

    /// <summary>
    /// RextHttpCongifuration class
    /// </summary>
    public class RextHttpCongifuration
    {
        /// <summary>
        /// Set the base url for every http call
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Set a proxy address when behind a corporate network eg: http://127.0.0.1:80. Mostly valid for development mode. Value should be passed from a dynamic setting
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Set a default header for every http call via IDictionary(string, string) IList(string, string) or key-value object (new { Authorization = "xxxx" }
        /// </summary>
        public object Header { get; set; }

        /// <summary>
        /// If set to true, httpclient will be configured to ignore SSL validations. This is useful when using selfsigned certificates
        /// </summary>
        public bool RelaxSslCertValidation { get; set; }

        /// <summary>
        /// If set to true, an exception is thrown whenever httpclient returns a statuscode other than 200
        /// </summary>
        public bool ThrowExceptionIfNotSuccessResponse { get; set; }

        /// <summary>
        /// If set to true, an exception is thrown whenever a response deserialization fails
        /// </summary>
        public bool ThrowExceptionOnDeserializationFailure { get; set; }

        /// <summary>
        /// If set to true, generic response will be deserialized only when status code is 200*. Default value is true
        /// </summary>
        public bool DeserializeSuccessResponseOnly { get; set; } = true;

        /// <summary>
        /// Http timeout in seconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Add a X509Certificate to the HttpClient
        /// </summary>
        public CertificateInfo Certificate { get; set; }

        /// <summary>
        /// Set the global HttpClient HttpCompletionOption call for every call. Default value is HttpCompletionOption.ResponseContentRead
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; }

        /// <summary>
        /// Change the default XML encoding format. Default is UTF-8
        /// </summary>
        public string DefaultXmlEncoding { get; set; } = "UTF-8";

        /// <summary>
        /// Configure JSON serializer settings
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }

    /// <summary>
    /// RextOptions class
    /// </summary>
    public class RextOptions
    {
        /// <summary>
        /// Request url in full
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Http verb to use, e.g GET, POST, DELETE
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Set a default header for every http call via IDictionary(string, string), IList(string, string) or key-value object (new { Authorization = "xxxx" }
        /// </summary>
        public object Header { get; set; }

        /// <summary>
        /// The data format of your payload
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The expected data format of the response
        /// </summary>
        public string ExpectedResponseFormat { get; set; }

        /// <summary>
        /// If set to true, a exception is thrown whenever httpclient returns a statuscode other than 200
        /// </summary>
        public bool? ThrowExceptionIfNotSuccessResponse { get; set; }

        /// <summary>
        /// If set to true, an exception is thrown whenever a response deserialization fails
        /// </summary>
        public bool? ThrowExceptionOnDeserializationFailure { get; set; }

        /// <summary>
        /// If set to true, generic response will be deserialized only when status code is 200*. Default global configuration value is true
        /// </summary>
        public bool? DeserializeSuccessResponseOnly { get; set; }

        /// <summary>
        /// Set to true if you are expecting Http response
        /// </summary>
        public bool IsStreamResponse { get; set; }

        internal bool IsForm { get; set; }

        internal bool IsUrlEncoded { get; set; }

        /// <summary>
        /// Set the HttpClient HttpCompletionOption call. Default value is HttpCompletionOption.ResponseContentRead
        /// </summary>
        public HttpCompletionOption? HttpCompletionOption { get; set; }

        /// <summary>
        /// Ignore certain status code set in resiliency policies
        /// </summary>
        public int[] IgnoreStatusCodeInResiliencyPolicies { get; set; } = Array.Empty<int>();
    }
}