using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Rext
{
    public static class HeaderExtension
    {
        public static IRextHttpClient AddHeader(this IRextHttpClient client, string key, string value)
        {
            client.Headers.Add(key, value);

            return client;
        }

        public static IRextHttpClient AddHeader(this IRextHttpClient client, IDictionary<string, string> headers)
        {
            foreach (var i in headers)
            {
                client.Headers.Add(i.Key, i.Value);
            }

            return client;
        }

        public static IRextHttpClient UseBearerAuthentication(this IRextHttpClient client, string token)
        {
            client.Headers.Add("Authorization", $"Bearer {token}");

            return client;
        }

        public static IRextHttpClient UseBasicAuthentication(this IRextHttpClient client, string username, string password)
        {
            // encode credentials
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            client.Headers.Add("Authorization", $"Basic {credentials}");

            return client;
        }

        public static void SetHeader(this HttpRequestMessage requestObj, object header)
        {
            //todo: review reflection process

            if (header != null)
            {
                // process from a list of objects
                if (header.IsList())
                {
                    foreach (var i in header as List<object>)
                    {
                        PropertyInfo headerItem = i.GetType().GetProperties().FirstOrDefault();
                        string value = headerItem.GetValue(i, null)?.ToString();

                        RemoveDuplicate(requestObj, headerItem.Name);

                        if (!string.IsNullOrEmpty(value)) // prevent adding null header item
                            requestObj.Headers.Add(headerItem.Name, value);
                    }
                }

                // process from a dictionary or key-value-pair
                else if (header.IsDictionary())
                {
                    foreach (var i in header as Dictionary<string, string>)
                    {
                        RemoveDuplicate(requestObj, i.Key);

                        if (!string.IsNullOrEmpty(i.Value))  // prevent adding null header item
                            requestObj.Headers.Add(i.Key, i.Value);
                    }
                }

                // process from a single object
                else
                {
                    PropertyInfo headerItem = header.GetType().GetProperties().FirstOrDefault();
                    string value = headerItem.GetValue(header, null)?.ToString();

                    RemoveDuplicate(requestObj, headerItem.Name);

                    if (!string.IsNullOrEmpty(value))  // prevent adding null header item
                        requestObj.Headers.Add(headerItem.Name, value);
                }
            }
        }

        private static void RemoveDuplicate(HttpRequestMessage requestObj, string headerKey)
        {
            // prevent duplicate header item
            if (requestObj.Headers.Contains(headerKey))
                requestObj.Headers.Remove(headerKey);
        }
    }
}