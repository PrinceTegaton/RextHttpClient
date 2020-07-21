using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;

namespace Rext
{
    internal static class Helpers
    {
        public static Uri CreateUri(this RextOptions options, string baseUrl = null)
        {
            // construct uri
            string url = string.Empty;
            string queryString = string.Empty;

            if (!string.IsNullOrEmpty(baseUrl))
            {
                if (options.Url.StartsWith("http://") || options.Url.StartsWith("https://"))
                    throw new UriFormatException("Invalid url format. When using BaseUrl you only have to supply the url part");

                // trim / from end and start to avoid double / in url
                url = $"{baseUrl.TrimEnd('/')}/{options.Url.TrimStart('/')}";
            }
            else
            {
                // use options.Url
                url = options.Url;
            }

            if (!url.StartsWith("http"))
                throw new UriFormatException("Invalid url format. Url scheme is required e.g. HTTP or HTTPS");

            // generate querystring from object if GET
            if (options.Method == HttpMethod.Get && options.Payload != null)
                queryString = options.Payload.ToQueryString();

            Uri uri = new Uri(url);

            string port = uri.Port > 0 ? ":" + uri.Port : string.Empty;
            if (!string.IsNullOrEmpty(uri.Query) && !string.IsNullOrEmpty(queryString))
                queryString = $"&{queryString?.TrimStart('?')}";

            uri = new Uri($"{uri.Scheme}://{uri.Host}{port}{uri.PathAndQuery}{queryString}");

            return uri;
        }
       
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

        public static (bool status, string message, T result) DeserializeJSON<T>(string content, bool throwExceptionOnDeserializationFailure = false)
        {
            try
            {
                // deserialize object to type T
                var obj = JsonConvert.DeserializeObject<T>(content);
                return (true, "OK", obj);
            }
            catch (Exception)
            {
                string msg = string.Format(StaticMessages.DeserializationFailure, typeof(T).Name);
                if (throwExceptionOnDeserializationFailure)
                    throw new RextException(msg); // throw exception as required
                else
                    return (false, msg, default(T)); // return failure message as required
            }
        }

        public static (bool status, string message, T result) DeserializeXML<T>(string content, bool throwExceptionOnDeserializationFailure = false)
        {
            try
            {
                // deserialize object to type T
                using (var stringReader = new StringReader(content))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var obj = serializer.Deserialize(stringReader);
                    return (true, "OK", (T)obj);
                }
            }
            catch (Exception)
            {
                string msg = string.Format(StaticMessages.DeserializationFailure, typeof(T).Name);
                if (throwExceptionOnDeserializationFailure)
                    throw new RextException(msg); // throw exception as required
                else
                    return (false, msg, default(T)); // return failure message as required
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

        public static string ToXml(this object value)
        {
            using (var writer = new StringWriter())
            {
                // empty default xml namespace
                // from the generated string

                var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, ns);

                // remove first xml line <?xml version="1.0" encoding="utf-16"?>
                // which is usually the first line but makes the request to contain 2 nodes

                if (!string.IsNullOrEmpty(writer.ToString()))
                {
                    var lines = writer.ToString().Split(Environment.NewLine.ToCharArray());
                    string xml = string.Empty;

                    for (int i = 1; i < lines.Length; i++) // i = 1 to skip <?xml on line 0
                        xml += lines[i]; // + Environment.NewLine;

                    return xml;
                }

                return string.Empty;
            }
        }
    }
}
