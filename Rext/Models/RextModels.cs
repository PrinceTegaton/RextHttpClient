﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Rext
{
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
        /// Execute action before any http call
        /// </summary>
        public Action BeforeCall { get; set; }

        /// <summary>
        /// Execute action after any http call
        /// </summary>
        public Action AfterCall { get; set; }

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
        /// Control Http Stopwatch from being executed. Value is True by default
        /// </summary>
        public bool EnableStopwatch { get; set; } = true;
    }

    public class RextHttpCongifuration
    {
        /// <summary>
        /// Set the base url for every call
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Set a proxy address when behind a corporate network eg: http://127.0.0.1:80. Mostly valid for development mode. Value should be passed from a dynamic setting
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }
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
        /// Http timeout in seconds
        /// </summary>
        public int Timeout { get; set; }
    }

    public class RextOptions
    {
        /// <summary>
        /// Request url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Http verb to use, e.g GET, POST, DELETE
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Content to post. This can be a simple or complex object
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }
        /// </summary>
        public object Header { get; set; }

        /// <summary>
        /// Httpclient request content-type
        /// </summary>
        //public string ContentType { get; set; }

        /// <summary>
        /// If set to true, a exception is thrown whenever httpclient returns a statuscode other than 200
        /// </summary>
        public bool? ThrowExceptionIfNotSuccessResponse { get; set; }

        /// <summary>
        /// If set to true, an exception is thrown whenever a response deserialization fails
        /// </summary>
        public bool? ThrowExceptionOnDeserializationFailure { get; set; }
    }
    
}