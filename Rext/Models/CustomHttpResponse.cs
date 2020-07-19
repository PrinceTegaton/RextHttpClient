using System.Net;

namespace Rext
{
    /// <summary>
    /// Custom Rext response for every http call
    /// </summary>
    public class CustomHttpResponse
    {
        /// <summary>
        /// This is true if the http repsonse code is 200
        /// </summary>
        public bool IsSuccess => StatusCode == HttpStatusCode.OK;

        /// <summary>
        /// The Http StatusCode associated with the call response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Plain string response from the http call
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Rext message on the status of the request and also shows handled exception messages
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Generic response that allows object to be deserialized to specified type of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomHttpResponse<T> : CustomHttpResponse
    {
        /// <summary>
        /// Generic type of T for response to be deserialized
        /// </summary>
        public T Data { get; set; }
    }
}
