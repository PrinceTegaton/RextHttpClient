using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rext
{
    public class CustomResponse
    {
        public bool IsSuccess
        {
            get
            {
                return StatusCode == HttpStatusCode.OK;
            }
        }
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
        public string Message { get; set; }
    }

    public class CustomHttpResponse<T> : CustomResponse
    {
        public T Data { get; set; }
    }
}
