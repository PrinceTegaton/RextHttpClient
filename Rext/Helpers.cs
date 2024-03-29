﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Rext
{
    internal static class Helpers
    {

        public static Uri CreateUri(this RextOptions options, string baseUrl = null)
        {
            // Validate the URL format if baseUrl is provided
            if (!string.IsNullOrEmpty(baseUrl))
            {
                if (options.Url.StartsWith("http://") || options.Url.StartsWith("https://"))
                    throw new UriFormatException("Invalid URL format. When using baseUrl, only supply the URL part.");
                
                options.Url = $"{baseUrl.TrimEnd('/')}/{options.Url.TrimStart('/')}";
            }

            // Validate the URL scheme
            if (!options.Url.StartsWith("http://") && !options.Url.StartsWith("https://"))
            {
                throw new UriFormatException("Invalid URL format. URL scheme is required, e.g., HTTP or HTTPS.");
            }

            // Generate the query string for GET requests
            string queryString = string.Empty;
            if (options.Method == HttpMethod.Get && options.Payload != null)
                queryString = options.Payload.ToQueryString();


            // Combine the base URL and query string to create the final URI
            Uri uri = new Uri(options.Url);

            string port = uri.Port > 0 ? ":" + uri.Port : string.Empty;
            if (!string.IsNullOrEmpty(uri.Query) && !string.IsNullOrEmpty(queryString))
                queryString = $"&{queryString?.TrimStart('?')}";
           
            uri = new Uri($"{uri.Scheme}://{uri.Host}{port}{uri.PathAndQuery}{queryString}");

            return uri;
        }

        public static bool IsList(this object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Type type = obj.GetType();
            return obj is IList && type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>));
        }

        public static bool IsDictionary(this object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Type type = obj.GetType();
            return obj is IDictionary && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        public static string ToQueryString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

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
                {
                    throw new RextException(msg); // throw exception as required
                }
                else
                {
                    return (false, msg, default(T)); // return failure message as required
                }
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
                {
                    throw new RextException(msg); // throw exception as required
                }
                else
                {
                    return (false, msg, default(T)); // return failure message as required
                }
            }
        }

        public static string ToJson(this object value, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (value == null)
            {
                return "{ }";
            }

            jsonSerializerSettings ??= new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
        }

        public static string ToXml(this object value, string encoding = "utf-8")
        {
            // create a blank namespace
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(value.GetType());
            var sb = new StringBuilder();

            using XmlWriter xw = XmlWriter.Create(sb);
            serializer.Serialize(xw, value, ns);
            xw.Flush();

            string xml = sb.ToString();

            if (encoding.ToLower() == "utf-8" && xml.Contains("utf-16"))
            {
                xml = xml.Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
            }

            return xml;
        }
    }
}
