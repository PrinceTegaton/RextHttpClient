using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rext
{
    internal class ProxyHttpClientHandler
    {
        public static HttpClientHandler ProxyHandler(string address = null, bool relaxSslCertValidation = false, CertificateInfo certificateInfo = null)
        {
            HttpClientHandler proxyHandler = new HttpClientHandler();

            if (!string.IsNullOrEmpty(address))
            {
                if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
                {
                    proxyHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(new Uri(address), BypassOnLocal: false),
                        UseProxy = true,
                        DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials
                    };
                }
                else
                {
                    throw new RextException("Invalid Proxy Url");
                }
            }
           

            if (relaxSslCertValidation)
                proxyHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; };

            // add certificate
            if (certificateInfo != null)
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert;

                // create cert from certificate filepath.pfx
                if (!string.IsNullOrEmpty(certificateInfo.FilePath))
                {
                    if (string.IsNullOrEmpty(certificateInfo.Password))
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath);
                    else
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath, certificateInfo.Password);

                    proxyHandler.ClientCertificates.Add(cert);
                    proxyHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return proxyHandler;
                }

                // create cert from certificate content bytes
                if (certificateInfo.CertificateBytes.Length > 0)
                {
                    if (string.IsNullOrEmpty(certificateInfo.Password))
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes);
                    else
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes, certificateInfo.Password);

                    proxyHandler.ClientCertificates.Add(cert);
                    proxyHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return proxyHandler;
                }

            }

            return proxyHandler;
        }
    }
}
