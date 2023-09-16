using System;
using System.Net;
using System.Net.Http;

namespace Rext
{
    internal class CustomHttpClientHandler
    {
        public static HttpClientHandler CreateHandler(string proxyAddress = null, bool relaxSslCertValidation = false, CertificateInfo certificateInfo = null)
        {
            var customHandler = new HttpClientHandler();

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                if (Uri.IsWellFormedUriString(proxyAddress, UriKind.Absolute))
                {
                    customHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(new Uri(proxyAddress), BypassOnLocal: false),
                        UseProxy = true,
                        DefaultProxyCredentials = CredentialCache.DefaultNetworkCredentials
                    };
                }
                else
                {
                    throw new RextException("Invalid Proxy Url");
                }
            }

            if (relaxSslCertValidation)
            {
                customHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; };
            }

            // add certificate
            if (certificateInfo != null)
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert;

                // create cert from certificate filepath.pfx
                if (!string.IsNullOrEmpty(certificateInfo.FilePath))
                {
                    if (string.IsNullOrEmpty(certificateInfo.Password))
                    {
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath);
                    }
                    else
                    {
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath, certificateInfo.Password);
                    }

                    customHandler.ClientCertificates.Add(cert);
                    customHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return customHandler;
                }

                // create cert from certificate content bytes
                if (certificateInfo.CertificateBytes.Length > 0)
                {
                    if (string.IsNullOrEmpty(certificateInfo.Password))
                    {
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes);
                    }
                    else
                    {
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes, certificateInfo.Password);
                    }

                    customHandler.ClientCertificates.Add(cert);
                    customHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return customHandler;
                }
            }

            return customHandler;
        }
    }
}
