using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rext
{
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

        void Dispose();

        /// <summary>
        /// Get XML result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> GetXML<T>(RextOptions options);

        /// <summary>
        /// Get XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> GetXML<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Get JSON result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options);

        /// <summary>
        /// Get JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Get plain string result. Accepts advanced options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<string>> GetString(RextOptions options);

        /// <summary>
        /// get plain string result
        /// </summary>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<string>> GetString(string url, object payload = null, object header = null);

        /// <summary>
        /// Make http request and receive result as string
        /// </summary>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns></returns>
        Task<CustomHttpResponse<string>> MakeRequest(RextOptions options);

        /// <summary>
        /// Make http request and receive result deserialized to custom type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options">RextOption to configure http call</param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> MakeRequest<T>(RextOptions options);

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(RextOptions options);

        /// <summary>
        /// Post JSON content for JSON result deserialized to custom type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> PostJSON<T>(string url, object payload = null, object header = null);

        /// <summary>
        /// Post XML content for JSON result deserialized to custom type. Accepts advanced options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> PostXML<T>(RextOptions options);

        /// <summary>
        /// Post XML content for XML result deserialized to custom type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        Task<CustomHttpResponse<T>> PostXML<T>(string url, object payload = null, object header = null);
    }
}
