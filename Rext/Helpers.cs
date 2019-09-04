using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;

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

        public static (bool status, string message, T result) DeserializeObject<T>(string content, bool throwExceptionOnDeserializationFailure = false) //, Action callbackOnException = null)
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
            using (var writer = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }
    }
}
