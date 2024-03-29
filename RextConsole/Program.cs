﻿using Rext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace RextConsole
{
    class Program
    {
        static IRextHttpClient _rext;

        static void Main(string[] args)
        {
            Console.WriteLine("Rext Console");

        restart:
            var r = _rext.DeleteJSON<dynamic>("https://localhost:44365/api/user/put", new { id = 1001, name = "john.doe" }).Result;
            Console.WriteLine($"{r.StatusCode} - {r.Message} \n{r.Content} {_rext.Stopwatch.ElapsedMilliseconds.ToString("N4")}ms");

            if (Console.ReadLine() == "R")
            {
                goto restart;
            }

            Console.ReadKey();
        }


        static void Main3(string[] args)
        {
            Console.WriteLine("Checking Rext performance to GET postman-echo.com/get?foo1=bar1&foo2=bar2 \n============================");
            _rext = new RextHttpClient();

        restart:
            var s = new Stopwatch();
            s.Start();

            for (int i = 0; i < 10; i++)
            {
                var r = _rext.GetString(new RextOptions
                {
                    Url = "https://postman-echo.com/get?foo1=bar1&foo2=bar2",
                    ContentType = "application/json"
                }).Result;

                Console.WriteLine($"{i + 1}. {r.StatusCode} {_rext.Stopwatch.ElapsedMilliseconds.ToString("N4")}ms");
            }

            s.Stop();
            Console.WriteLine($"Total time: {s.ElapsedMilliseconds.ToString("N4")}ms");
            if (Console.ReadLine() == "R")
            {
                goto restart;
            }

            Console.ReadKey();
        }

        static void Main2(string[] args)
        {
            try
            {
                RextHttpClient.Setup(opt =>
                {
                    opt.HttpConfiguration = new RextHttpCongifuration
                    {
                        //BaseUrl = "localhost:44316/api/home",
                        //ProxyAddress = "http://127.0.0.1:80",
                        ThrowExceptionOnDeserializationFailure = false,
                        ThrowExceptionIfNotSuccessResponse = false,
                        RelaxSslCertValidation = true
                        //Timeout = 60
                    };
                    opt.SuppressRextExceptions = false;
                    opt.BeforeCall = delegate ()
                    {
                        Console.WriteLine("---About to initiate http task");
                    };
                    opt.AfterCall = delegate (string url, CustomHttpResponse rsp)
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



                    var rsp = _rext.GetJSON<dynamic>(url1).GetAwaiter().GetResult();
                    //var rsp = _rext.GetString("https://localhost:44316/api/home/getstring").GetAwaiter().GetResult();
                    Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    Console.WriteLine(rsp.Data);

                    //var rsp = _rext.GetJSON<object>("https://localhost:44316/api/home/status?code=401", null, new { api_key = "12345" }).GetAwaiter().GetResult();

                    //var rsp = _rext.GetJSON<object>("getperson?id=1001", new { name = "joe" }, new { api_key = "12345" }).GetAwaiter().GetResult();

                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch?.ElapsedMilliseconds}ms");
                    //Console.WriteLine(rsp?.Data);

                    var p = new Person
                    {
                        Name = "Vicky Kay",
                        Location = "London, UK",
                        Status = false,
                        NextOkKin = new Person
                        {
                            Name = "William",
                            Location = "Lisbon",
                            Status = true
                        }
                    };

                    //var rsp = _rext.PostXML<Person>(new RextOptions
                    //{
                    //    Url = "https://localhost:44316/api/home/createperson",
                    //    Payload = p
                    //}).GetAwaiter().GetResult();
                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    //Console.WriteLine($"Name: {rsp.Data.Name} - Location: {rsp.Data.Location}");

                    //var rsp = _rext.GetXML<ArrayOfPerson>("https://localhost:44316/api/home/getpeoplelist")
                    //    .GetAwaiter().GetResult();
                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");

                    //foreach (var i in rsp.Data.Person)
                    //{
                    //    Console.WriteLine($"Name: {i.Name}, Location: {i.Location}");
                    //}

                    //var rsp = _rext.PostForm<Person>("https://localhost:44316/api/home/createpersonform", p)
                    //    .GetAwaiter().GetResult();
                    //Console.WriteLine($"{rsp.StatusCode} - {rsp.Message} - Duration: {_rext.Stopwatch.ElapsedMilliseconds}ms");
                    //Console.WriteLine($"Name: {rsp.Data.Name} - Location: {rsp.Data.Location}");

                    //var headers = new Dictionary<string, string>
                    //{
                    //    { "header1", "value 1" },
                    //    { "header2", "value 2" }
                    //};

                    //var rsp = _rext.AddHeader(headers)
                    //               .AddHeader("header3", "value 3")
                    //               .UseBearerAuthentication("ueyuywyt.iduizcg0e.kiuwnk==")
                    //               .UseBasicAuthentication("api_username", "api_passkey")
                    //               .PostJSON<@Person>(new RextOptions
                    //               {
                    //                    Url = "http://myapp.com/api/employee/getemployee",
                    //                    ContentType = "application/xml",
                    //                    Header = new { header4 = "value4" }
                    //               });

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
            {
                Console.WriteLine($"---handling Unauthorized Error");
            }

            if (RextHttpClient.ReturnStatusCode == 500)
            {
                Console.WriteLine($"---handling Internal Server Error");
            }
        }

        static void ErrorCodeHandler(int code)
        {
            if (code == 401)
            {
                Console.WriteLine($"---handling Unauthorized Error");
            }

            if (code == 500)
            {
                Console.WriteLine($"---handling Internal Server Error");
            }
        }
    }

    [XmlRoot(ElementName = "ArrayOfPerson")]
    public class ArrayOfPerson
    {
        [XmlElement(ElementName = "Person")]
        public List<Person> Person { get; set; }
    }

    [XmlRoot(ElementName = "Person")]
    public class Person
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Location")]
        public string Location { get; set; }
        [XmlElement(ElementName = "Status")]
        public bool Status { get; set; }

        [XmlElement(ElementName = "NextOfKin")]
        public Person NextOkKin { get; set; }
    }
}
