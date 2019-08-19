using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rext
{
    public static class Helpers
    {
        public static T DeserializeObject<T>(string content, bool throwExceptionOnFailure = false) // where T : class
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(content);
                return obj;
            }
            catch (Exception)
            {
                throw new RextException($"Unable to deserialize object to specified type of '{nameof(T)}'");
                //{ex?.InnerException?.Message ?? ex?.Message});
            }
        }

        public static string ToJson(this object value)
        {
            if (value == null) return "{ }";
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, settings);
        }
    }

    public class ProxyHttpClientHandler
    {
        public static HttpClientHandler ProxyHandler(string address)
        {
            var proxyHandler = new HttpClientHandler
            {
                Proxy = string.IsNullOrEmpty(address) ? null : new WebProxy(new Uri(address), BypassOnLocal: false),
                UseProxy = string.IsNullOrEmpty(address) ? false : true,
                DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            return proxyHandler;
        }
    }

    public class RextException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public RextException(string message) : base(message)
        {

        }

        public RextException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
