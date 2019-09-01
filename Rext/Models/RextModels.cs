using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Rext
{
    public class RextHttpCongifuration
    {
        public string BaseUrl { get; set; }
        public string ProxyAddress { get; set; }
        public object Header { get; set; }
        public bool RelaxSslCertValidation { get; set; }
        public bool ThrowExceptionIfNotSuccessResponse { get; set; }
        public bool ThrowExceptionOnDeserializationFailure { get; set; }
    }

    public class RextOptions
    {
        public string Url { get; set; }
        public object Payload { get; set; }
        public IEnumerable<object> Header { get; set; }
        public bool ThrowExceptionIfNotSuccessResponse { get; set; }
        public bool ThrowExceptionOnDeserializationFailure { get; set; }
    }
}