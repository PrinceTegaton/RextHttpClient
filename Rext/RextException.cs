using System;
using System.Net;

namespace Rext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RextException"/> class
    /// </summary>
    public class RextException : Exception
    {
        /// <summary>
        /// Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="message"></param>
        public RextException(string message) : base(message)
        {

        }

        /// <summary>
        /// Rext custom error handler
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <param name="statusCode">HTTP status code</param>
        public RextException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
