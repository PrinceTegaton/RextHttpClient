using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
        /// Get execution time of the http call
        /// </summary>
        Stopwatch Stopwatch { get; }

        /// <summary>
        /// Get XML result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options);

        /// <summary>
        /// Get XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetXML<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Get JSON result deserialized to custom type. Accepts advanced options.
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options);

        /// <summary>
        /// Get JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Get plain string result. Accepts advanced options
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>String response</returns>
        Task<CustomHttpResponse<string>> GetString(RextOptions options);

        /// <summary>
        /// get plain string result
        /// </summary>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>String response</returns>
        Task<CustomHttpResponse<string>> GetString(string url, object payload = null, object header = null);

        /// <summary>
        /// Make http request and receive result as string
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>String content for your consumption as required</returns>
        Task<CustomHttpResponse<string>> MakeRequest(RextOptions options);

        /// <summary>
        /// Make http request and receive result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options);

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options);

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Post XML content for JSON result deserialized to custom type. Accepts advanced options. You can change request format with RextOptions.ContentType
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options);

        /// <summary>
        /// Post XML content for XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostXML<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostForm<T>(RextOptions options, bool isUrlEncoded = false);

        /// <summary>
        /// Post content as form-data for JSON result deserialized to custom type. Uses multipart/form-data by default
        /// </summary>
        /// <typeparam name="T">Generic return type to deserialize response data in to</typeparam>
        /// <param name="url">Request url in full</param>
        /// <param name="payload">Content to send. Can be a single/complex object, list, keyvalue pair or more, depending on the api request</param>
        /// <param name="header">Set a default header for every http call via IDictionary<string, string>, IList<string, string> or key-value object (new { Authorization = "xxxx" }</param>
        /// <param name="isUrlEncoded">Set to true to send as application/x-www-form-urlencoded</param>
        /// <returns>Deserialized response of T</returns>
        Task<CustomHttpResponse<T>> PostForm<T>(string url, object payload = null, object header = null, bool isUrlEncoded = false);
    }
}
