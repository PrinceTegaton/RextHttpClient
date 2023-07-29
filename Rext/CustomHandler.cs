using System;
using System.Net;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;

namespace Rext
{
    internal class CustomHttpClientHandler
    {
        public static HttpClientHandler CreateHandler(string proxyAddress = null, bool relaxSslCertValidation = false, CertificateInfo certificateInfo = null)
        {
            HttpClientHandler customHandler = new HttpClientHandler();

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                if (Uri.IsWellFormedUriString(proxyAddress, UriKind.Absolute))
                    customHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(new Uri(proxyAddress), BypassOnLocal: false),
                        UseProxy = true,
                        DefaultProxyCredentials = CredentialCache.DefaultNetworkCredentials
                    };
                else
                    throw new RextException("Invalid Proxy Url");
            }

            if (relaxSslCertValidation)
                customHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return relaxSslCertValidation; };


            // add certificate
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert;

            // create cert from certificate filepath.pfx
            if (!string.IsNullOrEmpty(certificateInfo.FilePath))
            {
                cert = (!string.IsNullOrEmpty(certificateInfo.FilePath))
                        ? new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath)
                        : new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.FilePath, certificateInfo.Password);

                customHandler.ClientCertificates.Add(cert);
            }

            // create cert from certificate content bytes
            if (certificateInfo.CertificateBytes.Length > 0)
            {
                cert = (string.IsNullOrEmpty(certificateInfo.Password))
                    ? new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes)
                    : new System.Security.Cryptography.X509Certificates.X509Certificate2(certificateInfo.CertificateBytes, certificateInfo.Password);

                customHandler.ClientCertificates.Add(cert);
            }

            customHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            return customHandler;
        }
    }
}