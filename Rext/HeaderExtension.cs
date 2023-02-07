using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Rext
{
    /// <summary>
    /// Header extension
    /// </summary>
    public static class HeaderExtension
    {
        /// <summary>
        /// Add a single header item with key and value
        /// </summary>
        /// <param name="client"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IRextHttpClient AddHeader(this IRextHttpClient client, string key, string value)
        {
            if (client.Headers.ContainsKey(key))
            {
                client.Headers.Remove(key);
            }

            client.Headers.TryAdd(key, value);

            return client;
        }

        /// <summary>
        /// Add a Dictionary of headers
        /// </summary>
        /// <param name="client"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static IRextHttpClient AddHeader(this IRextHttpClient client, IDictionary<string, string> headers)
        {
            foreach (var i in headers)
            {
                if (client.Headers.ContainsKey(i.Key))
                {
                    client.Headers.Remove(i.Key);
                }

                client.Headers.TryAdd(i.Key, i.Value);
            }

            return client;
        }

        /// <summary>
        /// Use Bearer Authentication by supplying just the token
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token">API bearer token</param>
        /// <returns></returns>
        public static IRextHttpClient UseBearerAuthentication(this IRextHttpClient client, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                client.Headers.TryAdd("Authorization", $"Bearer {token}");
            }

            return client;
        }

        /// <summary>
        /// Use Basic Authentication by supplying just the username and password
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username">API username</param>
        /// <param name="password">API password</param>
        /// <returns></returns>
        public static IRextHttpClient UseBasicAuthentication(this IRextHttpClient client, string username, string password)
        {
            // encode credentials
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                client.Headers.TryAdd("Authorization", $"Basic {credentials}");
            }

            return client;
        }

        internal static void SetHeader(this HttpRequestMessage requestObj, object header)
        {
            //todo: review reflection process

            if (header == null)
            {
                return;
            }

            // process from a list of objects
            if (header.IsList())
            {
                foreach (var i in header as List<object>)
                {
                    PropertyInfo headerItem = i.GetType().GetProperties().FirstOrDefault();
                    string value = headerItem?.GetValue(i, null)?.ToString();

                    RemoveDuplicate(requestObj, headerItem?.Name);

                    if (!string.IsNullOrEmpty(value)) // prevent adding null header item
                    {
                        requestObj.Headers.TryAddWithoutValidation(headerItem.Name, value);
                    }
                }
            }

            // process from a dictionary or key-value-pair
            else if (header.IsDictionary())
            {
                foreach (var i in header as Dictionary<string, string>)
                {
                    RemoveDuplicate(requestObj, i.Key);

                    if (!string.IsNullOrEmpty(i.Value))  // prevent adding null header item
                    {
                        requestObj.Headers.TryAddWithoutValidation(i.Key, i.Value);
                    }
                }
            }

            // process from a single object
            else
            {
                PropertyInfo headerItem = header.GetType().GetProperties().FirstOrDefault();
                string value = headerItem?.GetValue(header, null)?.ToString();

                RemoveDuplicate(requestObj, headerItem?.Name);

                if (!string.IsNullOrEmpty(value))  // prevent adding null header item
                {
                    requestObj.Headers.TryAddWithoutValidation(headerItem?.Name, value);
                }
            }
        }

        internal static void SetHeader(this HttpRequestMessage requestObj, string key, string value)
        {
            // prevent adding null header item
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    RemoveDuplicate(requestObj, key);
                    requestObj.Headers.TryAddWithoutValidation(key, value);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void RemoveDuplicate(HttpRequestMessage requestObj, string key)
        {
            // prevent duplicate header item
            try
            {
                if (requestObj.Headers.Contains(key))
                {
                    requestObj.Headers.Remove(key);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}