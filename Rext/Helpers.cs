using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rext
{
    public static class Helpers
    {
        public static bool IsList(this object obj)
        {
            if (obj == null) return false;

            Type type = obj.GetType();
            return obj is IList && type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>));
        }

        public static bool IsDictionary(this object obj)
        {
            if (obj == null) return false;

            Type type = obj.GetType();
            return obj is IDictionary && type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;

            Type type = obj.GetType();
            var props = type.GetProperties();
            string[] pairs = props.Select(x => x.Name + "=" + x.GetValue(obj, null)).ToArray();
            string queryString = "?" + string.Join("&", pairs);

            return queryString;
        }

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
