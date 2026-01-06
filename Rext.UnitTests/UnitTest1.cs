namespace Rext.UnitTests
{
    public class UnitTest1
    {
        private IRextHttpClient _rext;
        private string baseUrl => "https://api.restful-api.dev/";

        public UnitTest1 ()
        {
            _rext = new RextHttpClient();
        }

        [Fact]
        public async Task GetString_Valid()
        {
            var res = await _rext.GetJSON<dynamic>(baseUrl + "objects/7");
            Assert.True(res.IsSuccess);
        }
    }
}
