using Rext;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RextConsole
{
    class Program
    {
        static IRextHttpClient _rext;


        static void Main(string[] args)
        {
            try
            {
                RextHttpClient.Setup(opt =>
                {
                    opt.HttpConfiguration = new RextHttpCongifuration
                    {
                        ProxyAddress = "http://172.27.4.3:80",
                        ThrowExceptionOnDeserializationFailure = false,
                        ThrowExceptionIfNotSuccessResponse = false,
                        //Timeout = 60
                    };
                    opt.BeforeCall = delegate ()
                    {
                        Console.WriteLine("---About to initiate http task");
                    };
                    opt.AfterCall = delegate ()
                    {
                        Console.WriteLine("---End of http task");
                    };
                    opt.OnError = delegate ()
                    {
                        Console.WriteLine("---Error occured");
                    };
                    opt.OnStatusCode = (int code) => ErrorCodeHandler();
                    opt.StatusCodesToHandle = new int[] { 401, 500 };
                });

                _rext = new RextHttpClient();

                Console.WriteLine("Hello Rext!");

                string url1 = "https://fudhubapi.dynamicbra.in/api/product/getall";
                string url2 = "http://httpstat.us/500";
                var header = new Dictionary<string, string>
                {
                   { "client_secret", "xxx12345" },
                   { "role", "admin" }
                };

                var header_single = new { header_single = "header obj from GetJSON" };

                //var rsp = _rext.UseBasicAuthentication("user", "pwd")
                //               //.UseBearerAuthentication("877834780948087yihfsjhfjs==")
                //               .AddHeader(new Dictionary<string, string>
                //               {
                //                   { "item_1", "one" },
                //                   { "item_2", "one" }
                //               })
                //               .AddHeader("single_obj", "o_b_j")
                //               .GetJSON<string>(url,
                //                    new
                //                    {
                //                        name = "Jordan Pickford"
                //                    }, 
                //                    header_single).GetAwaiter().GetResult();

                

                var rsp = _rext.GetString(url2).GetAwaiter().GetResult();
                Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine(rsp.Data);

                var rsp2 = _rext.GetJSON<object>(new RextOptions
                {
                    Url = url1,
                    //ThrowExceptionOnDeserializationFailure = true
                })
                .GetAwaiter().GetResult();
                Console.WriteLine($"{rsp2.StatusCode} - {rsp2.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine(rsp2.Data);
                
                var rsp3 = _rext.GetXML<object>("https://api.beta.shipwire.com/exec/RateServices.php")
                .GetAwaiter().GetResult();
                Console.WriteLine($"{rsp3.StatusCode} - {rsp3.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine(rsp3.Data);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void ErrorCodeHandler()
        {
            if (RextHttpClient.ReturnStatusCode == 401)
                Console.WriteLine($"---handling Unauthorized Error");
            if (RextHttpClient.ReturnStatusCode == 500)
                Console.WriteLine($"---handling Internal Server Error");
        }

        static void ErrorCodeHandler(int code)
        {
            if (code == 401)
                Console.WriteLine($"---handling Unauthorized Error");
            if (code == 500)
                Console.WriteLine($"---handling Internal Server Error");
        }
    }
}
