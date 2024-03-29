﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Rext
{
    /// <summary>
    /// IRextHttpClient interface
    /// </summary>
    public interface IRextHttpClient
    {
        /// <summary>
        /// List of headers appended from HeaderExtension
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// List of resiliency policies
        /// </summary>
        List<ResiliencyPolicy> ResiliencyPolicies { get; }

        /// <summary>
        /// Get execution time of the http call when configured to run
        /// </summary>
        Stopwatch Stopwatch { get; }

        /// <summary>
        /// Set request timeout
        /// </summary>
        TimeSpan? Timeout { get; set; }



        /// <summary>
        /// Put JSON content for string result. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call using <see cref="RextOptions"/> object</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> MakeRequest(RextOptions options);

        /// <summary>
        /// Process direct request with plain string response. This method is called by all verb actions
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options);


        #region GET
        /// <summary>
        /// Get XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetXML<T>(string url, object payload = null, object header = null);


        /// <summary>
        /// Get XML result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options);

        /// <summary>
        /// Get JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Get JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object.
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options);

        /// <summary>
        /// get plain string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>String response</returns>
        Task<CustomHttpResponse<string>> GetString(string url, object payload = null, object header = null);

        /// <summary>
        /// Get plain string result. Accepts advanced options using <see cref="RextOptions"/> object
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>String response</returns>
        Task<CustomHttpResponse<string>> GetString(RextOptions options);
        #endregion

        #region POST
        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options);

        /// <summary>
        /// Post JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PostJSONForString(string url, object payload = null, object header = null);

        /// <summary>
        /// Post JSON content for string result. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PostJSONForString(RextOptions options);

        /// <summary>
        /// Post XML content for XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostXML<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Post XML content for JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options);

        /// <summary>
        /// Post XML content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PostXMLForString(string url, object payload = null, object header = null);

        /// <summary>
        /// Post XML content for string result. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PostXMLForString(RextOptions options);

        /// <summary>
        /// Post plain string content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Plain string response</returns>
        Task<CustomHttpResponse<string>> PostString(string url, object payload = null, object header = null);

        /// <summary>
        /// Post plain string content for string result. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Plain string response</returns>
        Task<CustomHttpResponse<string>> PostString(RextOptions options);

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostForm<T>(string url, object payload = null, object header = null, bool isUrlEncoded = false);

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostForm<T>(RextOptions options, bool isUrlEncoded = false);
        #endregion

        #region DELETE
        /// <summary>
        /// Delete for string result deserialized to custom type
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send will be serialized to querystring</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> Delete(string url, object payload = null, object header = null);

        /// <summary>
        /// Delete for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send will be serialized to querystring</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> DeleteJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Delete for JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call. Payload will be serialized to querystring</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> DeleteJSON<T>(RextOptions options);
        #endregion

        #region PUT

        /// <summary>
        /// Put JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PutJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Put JSON content for JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PutJSON<T>(RextOptions options);

        /// <summary>
        /// Put JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PutJSONForString(string url, object payload = null, object header = null);

        /// <summary>
        /// Put JSON content for string result. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PutJSONForString(RextOptions options);
        #endregion

        #region PATCH

        /// <summary>
        /// Patch JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PatchJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Patch JSON content for JSON result deserialized to custom type. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PatchJSON<T>(RextOptions options);

        /// <summary>
        /// Patch JSON content for string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary of strings, IList of string or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PatchJSONForString(string url, object payload = null, object header = null);

        /// <summary>
        /// Patch JSON content for string result. Accepts advanced options using <see cref="RextOptions"/> object. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<string>> PatchJSONForString(RextOptions options);
        #endregion
    }
}