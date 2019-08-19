using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rext
{


    public class RextOptions
    {
        public string Url { get; set; }
        public object Payload { get; set; }
        public HttpHeaders Header { get; set; };
        public bool ThrowExceptionIfNotSuccessResponse { get; set; }
        public bool ThrowExceptionOnDeserializationFailure { get; set; }
    }

    public class RextHttpClient
    {
        public string ProxyAddress { get; set; }

        public RextHttpClient(string proxyAddress = null)
        {
            ProxyAddress = proxyAddress;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(RextOptions options)
        {
            return GetJSON<T>(options.Url, options.Payload, options.Header).Result;
        }

        public async Task<CustomHttpResponse<T>> GetJSON<T>(string url, object param = null, object header = null)
        {
            var rsp = new CustomHttpResponse<T>();

            var data = await MakeRequest(HttpMethod.Get, url, param, header);
            if (data == null)
            {

            }

            rsp.StatusCode = data.StatusCode;

            if (data.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (!string.IsNullOrEmpty(data.Content))
                {
                    T newObj = Helpers.DeserializeObject<T>(data.Content, true);
                    rsp.Data = newObj;
                }

                return rsp;
            }

            return rsp;
        }

        public async Task<CustomHttpResponse<string>> MakeRequest(HttpMethod method, string url, object param = null, object header = null)
        {
            var rsp = new CustomHttpResponse<string>();
            var response = new HttpResponseMessage();
            string responseString = null;

            try
            {
                string queryString = string.Empty;

                if (method == HttpMethod.Get && param != null)
                {
                    var type = param.GetType();
                    var props = type.GetProperties();
                    var pairs = props.Select(x => x.Name + "=" + x.GetValue(param, null)).ToArray();
                    queryString = "?" + string.Join("&", pairs);
                }

                string endPoint = $"{queryString}";
                var requestUri = new Uri($"{url}{queryString}");
                var httpRequestMessage = new HttpRequestMessage(method, requestUri);

                // POST request
                if (method == HttpMethod.Post & param != null)
                {
                    string strPayload = param.ToJson();
                    httpRequestMessage.Content = new StringContent(strPayload, Encoding.UTF8, "application/json");
                }

                //httpRequestMessage.Headers. = new AuthenticationHeaderValue("amx", null);

                response = await this.SendAsync(httpRequestMessage, CancellationToken.None);
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
                    rsp.Message = "Http call unsuccessful";
                }

                //else
                //{
                //    responseString = await response.Content.ReadAsStringAsync();

                //    throw new RextException(responseString)
                //    {
                //        StatusCode = response.StatusCode
                //    };
                //}

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

            using (var httpClientHandler = ProxyHttpClientHandler.ProxyHandler(ProxyAddress))
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    response = await client.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }
}