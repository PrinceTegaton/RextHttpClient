using Rext.ApiSimulator;
using System.Net;

namespace Rext.UnitTests
{
    public class MiscellaneousTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        private IRextHttpClient _rext;

        public MiscellaneousTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7173/api/app");

            RextHttpClient.Setup(opt => 
            {
                opt.EnableStopwatch = true;
                opt.HttpConfiguration = new()
                {
                    ThrowExceptionIfNotSuccessResponse = true,
                    ThrowExceptionOnDeserializationFailure = true
                };
                opt.HttpClient = _client;
            });

            _rext = new RextHttpClient();
        }

        [Fact]
        public async Task GlobalConfigIntegrity()
        {
            Assert.Null(RextHttpClient.ConfigurationBundle.HttpConfiguration.BaseUrl);
            Assert.True(RextHttpClient.ConfigurationBundle.HttpConfiguration.ThrowExceptionIfNotSuccessResponse);
            Assert.True(RextHttpClient.ConfigurationBundle.HttpConfiguration.ThrowExceptionOnDeserializationFailure);
            Assert.True(RextHttpClient.ConfigurationBundle.EnableStopwatch);
        }

        [Fact]
        public async Task HandledError_500()
        {
            var res = await _rext.GetString("handlederror");
            Assert.False(res.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
            Assert.NotNull(res.Exception);
        }

        [Fact]
        public async Task UnhandledError_500()
        {
            var res = await _rext.GetString("unhandlederror");
            Assert.False(res.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
            Assert.NotNull(res.Exception);
        }
    }
}
