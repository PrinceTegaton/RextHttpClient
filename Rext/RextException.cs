using System;
using System.Net;

namespace Rext
{
    public class RextException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public RextException(string message) : base(message)
        {

        }

        public RextException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
