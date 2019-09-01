using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rext
{
    public class RextHttpClient
    {
        public RextHttpCongifuration Configuration { get; set; } = new RextHttpCongifuration();


        public RextHttpClient(RextHttpCongifuration configuration = null)
        {
            if (configuration != null)
                this.Configuration = configuration;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options)
        {
            return await GetJSON<T>(options.Url, options.Payload, options.Header);
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(string url, object payload = null, object header = null)
        {
            var data = await MakeRequest<T>(HttpMethod.Get, url, payload, header);
            if (data == null)
            {

            }

            return data;
        }

        public async Task<CustomHttpResponse<T>> MakeRequest<T>(HttpMethod method, string url, object param = null, object header = null)
        {
            var rsp = new CustomHttpResponse<T>();
            var response = new HttpResponseMessage();
            string responseString = string.Empty;

            try
            {
                string queryString = string.Empty;

                // generate querystring from object if GET
                if (method == HttpMethod.Get && param != null)
                    queryString = param.ToQueryString();

                HttpRequestMessage requestMsg = new HttpRequestMessage(method, new Uri(url + queryString));

                // set header if object has value
                if (header != null)
                    SetHeader(requestMsg, header);

                // POST request
                if (method == HttpMethod.Post & param != null)
                {
                    // convert object to JSON
                    string strPayload = param.ToJson();
                    requestMsg.Content = new StringContent(strPayload, Encoding.UTF8, "application/json");
                }

                response = await this.SendAsync(requestMsg, CancellationToken.None);
                rsp.StatusCode = response.StatusCode;
                responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    rsp.Content = responseString;
                    rsp.Message = "Http call successful";
                }
                else
                {
                    rsp.Content = responseString;
                    rsp.Message = "Http call completed but not successful";
                }

                if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        T newObj = Helpers.DeserializeObject<T>(responseString, true);
                        rsp.Data = newObj;
                    }
                }
                else
                {

                }

                return rsp;
            }
            catch (Exception ex)
            {
                rsp.StatusCode = response.StatusCode;
                //if (response.Content != null) await response?.Content?.ReadAsStringAsync();
                rsp.Message = /*responseString ??*/ ($"{ex?.Message} :: {ex?.InnerException?.Message}");
                //"Error :: Unable to get data at the moment!";
                return rsp;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            //string requestContentBase64String = string.Empty;
            //string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.ToString().ToLower());
            //string requestHttpMethod = request.Method.Method;

            using (var httpClientHandler = ProxyHttpClientHandler.ProxyHandler(Configuration.ProxyAddress, Configuration.RelaxSslCertValidation))
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    response = await client.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }

        private void SetHeader(HttpRequestMessage requestObj, object header)
        {
            if (header != null)
            {
                // process from a list of objects
                if (header.IsList())
                {
                    foreach (var i in header as List<object>)
                    {
                        PropertyInfo headerItem = i.GetType().GetProperties().FirstOrDefault();
                        string value = headerItem.GetValue(i, null)?.ToString();

                        if (!string.IsNullOrEmpty(value)) // prevent adding null header item
                            requestObj.Headers.Add(headerItem.Name, value);
                    }
                }

                // process from a dictionary or key-value-pair
                else if (header.IsDictionary())
                {
                    foreach (var i in header as Dictionary<string, string>)
                    {
                        if (!string.IsNullOrEmpty(i.Value))  // prevent adding null header item
                            requestObj.Headers.Add(i.Key, i.Value);
                    }
                }

                // process from a single object
                else
                {
                    PropertyInfo headerItem = header.GetType().GetProperties().FirstOrDefault();
                    string value = headerItem.GetValue(header, null)?.ToString();

                    if (!string.IsNullOrEmpty(value))  // prevent adding null header item
                        requestObj.Headers.Add(headerItem.Name, value);
                }
            }
        }
    }
}