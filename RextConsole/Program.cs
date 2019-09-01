using Rext;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RextConsole
{
    class Program
    {
        static Rext.RextHttpClient _rext;

        static void Main(string[] args)
        {
            try
            {
                _rext = new Rext.RextHttpClient();

                Console.WriteLine("Hello Rext!");

                string url = "http://localhost/ryderweb.client/api/auth/testbadrequest";
                var header = new Dictionary<string, string>
                {
                   { "Authorization", "Bearer xxx12345" },
                   { "Role", "admin" }
                };

                var header_single = new { Authorization =  "Bearer xxx12345" };

                var rsp = _rext.GetJSON<string>("http://localhost/ryderweb.client/api/auth/testbadrequest",
                    new { name = "Jordan Pickford" }, header_single).Result;

                if (rsp.IsSuccess)
                {
                    Console.WriteLine(rsp.Data.ToString());
                }
                else
                {
                    Console.WriteLine(rsp.StatusCode.ToString());
                    Console.WriteLine(rsp.Message);
                    Console.WriteLine(rsp.Data.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();

        }
    }
}
