using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rext
{
    public class ProxyHttpClientHandler
    {
        public static HttpClientHandler ProxyHandler(string address = null, bool relaxSslCertValidation = false)
        {
            HttpClientHandler proxyHandler = new HttpClientHandler();

            if (!string.IsNullOrEmpty(address))
            {
                proxyHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(new Uri(address), BypassOnLocal: false),
                    UseProxy = true,
                    DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; }
                };
            }

            if (relaxSslCertValidation)
                proxyHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; };

            return proxyHandler;
        }
    }
}
