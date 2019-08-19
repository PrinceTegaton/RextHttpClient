using Rext;
using System;
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

                var v = new RextOptions
                {
                    Url = "http://localhost/ryderweb.client/api/auth/testbadrequest",
                    Payload = new { }
                };

                var rsp = _rext.GetJSON<int>(new RextOptions
                {
                    Url = "http://localhost/ryderweb.client/api/auth/testbadrequest",
                    Payload = new { },
                    Header = 
                }).Result;
                if (rsp.IsSuccess)
                {
                    Console.WriteLine(rsp.Data.ToString());
                }
                else
                {
                    Console.WriteLine(rsp.StatusCode.ToString());
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
