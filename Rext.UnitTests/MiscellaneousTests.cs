using Microsoft.AspNetCore.Mvc.Testing;
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

            _rext = new RextHttpClient(
                configuration: new()
                {
                    ThrowExceptionIfNotSuccessResponse = true,
                    ThrowExceptionOnDeserializationFailure = true
                },
                httpClient: _client);
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
            RextHttpClient.ConfigurationBundle.HttpConfiguration.ThrowExceptionIfNotSuccessResponse = true;

            var res = await _rext.GetString("unhandlederror");
            Assert.False(res.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
            Assert.NotNull(res.Exception);
        }
    }
}
