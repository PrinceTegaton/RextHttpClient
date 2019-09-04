using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
