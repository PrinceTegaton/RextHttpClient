using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        public void GetClient()
        {
            var c = new HttpClient();
            var m = new HttpRequestMessage();
            var r = new HttpResponseMessage();
        }
    }
}
