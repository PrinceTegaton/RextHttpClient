using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rext;

namespace RextUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private IRextHttpClient _rext;
        private string _baseUrl => "https://reqres.in/api/x";

        [TestInitialize]
        public void Init()
        {
            _rext = new RextHttpClient();
        }

        [TestMethod]
        public void GetString_Valid()
        {
            var res = _rext.GetJSON<dynamic>(_baseUrl + "users/2").Result;
            Assert.IsTrue(res.IsSuccess);
        }
    }
}
