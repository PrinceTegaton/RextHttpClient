using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rext
{
    internal class CustomHttpClientHandler
    {
        public static HttpClientHandler CreateHandler(string address = null, bool relaxSslCertValidation = false, CertificateInfo certificateInfo = null)
        {
            HttpClientHandler customHandler = new HttpClientHandler();

            if (!string.IsNullOrEmpty(address))
            {
                if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
                {
                    customHandler = new HttpClientHandler
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
                customHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; };

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

                    customHandler.ClientCertificates.Add(cert);
                    customHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return customHandler;
                }

                // create cert from certificate content bytes
                if (certificateInfo.CertificateBytes.Length > 0)
                {
                    if (string.IsNullOrEmpty(certificateInfo.Password))
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes);
                    else
                        cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes, certificateInfo.Password);

                    customHandler.ClientCertificates.Add(cert);
                    customHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                    return customHandler;
                }

            }

            return customHandler;
        }
    }
}
