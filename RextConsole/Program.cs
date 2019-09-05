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

            _RunTest:
                Console.WriteLine("Hit 'R' to run test");

                if (Console.ReadLine().ToUpper() == "R")
                {
                    Console.Clear();
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


                    //var rsp = _rext.GetString("https://localhost:44316/api/home/getstring").GetAwaiter().GetResult();
                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    //Console.WriteLine(rsp.Data);

                    //var rsp = _rext.GetJSON<object>("https://localhost:44316/api/home/status?code=401", null, new { api_key = "12345" }).GetAwaiter().GetResult();
                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    //Console.WriteLine(rsp.Data);

                    var p = new Person
                    {
                        Name = "Jack",
                        Location = "Manchester",
                        Status = true
                    };

                    var rsp = _rext.PostXML<Person>(new RextOptions
                    {
                        Url = "https://localhost:44316/api/home/createperson",
                        Payload = p,
                        ContentType = ContentType.Application_JSON
                    }).GetAwaiter().GetResult();
                    Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    Console.WriteLine($"Name: {rsp.Data.Name} - Location: {rsp.Data.Location}");

                    Console.WriteLine("--------------");
                    goto _RunTest;
                }
                else
                {
                    Console.WriteLine("--------------");
                    Console.ReadKey();
                }
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

    public class Person
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
    }
}
