using Microsoft.AspNetCore.Mvc.Testing;
using Rext.ApiSimulator;
using System.Net;

namespace Rext.UnitTests
{
    public class UnitTest1 : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        private IRextHttpClient _rext;

        public UnitTest1(ApiFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7173/api");

            _rext = new RextHttpClient(
                configuration: new()
                {
                    ThrowExceptionIfNotSuccessResponse = false,
                    ThrowExceptionOnDeserializationFailure = false
                },
                httpClient: _client);
        }

        [Fact]
        public async Task GET_WithQuery()
        {
            var res = await _rext.GetJSON<Result<User>>("app/getbyquery?id=1");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Fact]
        public async Task GET_WithQuery_Fail_400()
        {
            var res = await _rext.GetJSON<Result<User>>("app/getbyquery?id=100");
            Assert.False(res.IsSuccess);
            Assert.Null(res.Data);
            Assert.True(res.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_WithPath()
        {
            var res = await _rext.GetJSON<Result<User>>("app/getbypath/1");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Fact]
        public async Task GET_WithPath_Fail_400()
        {
            var res = await _rext.GetJSON<Result<User>>("app/getbypath/150");
            Assert.False(res.IsSuccess);
            Assert.Null(res.Data);
            Assert.True(res.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_PlainString()
        {
            var res = await _rext.GetString("app/getbyquery?id=1");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.StartsWith("{", res.Data);
            Assert.EndsWith("}", res.Data);
        }

        [Fact]
        public async Task GET_List()
        {
            var res = await _rext.GetJSON<Result<IEnumerable<User>>>("app/getall");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data);
        }

        [Fact]
        public async Task GET_List_WithQuery_WithWrapper()
        {
            var res = await _rext.GetJSON<Result<IEnumerable<User>>>("app/getallwithquery?filter_name=carlos&withwrapper=true");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data);
        }

        [Fact]
        public async Task GET_List_WithQuery_WithoutWrapper()
        {
            var res = await _rext.GetJSON<IEnumerable<User>>("app/getallwithquery?filter_name=carlos&withwrapper=false");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotEmpty(res.Data);
        }

        [Fact]
        public async Task POST_Create()
        {
            var res = await _rext.PostJSON<Result<int>>("app/create",
                new User
                {
                    Name = "Orlando",
                    DateOfBirth = DateOnly.Parse("1991-03-12"),
                    Contact = new()
                    {
                        EmailAddress = "orlando.b@example.com",
                        MobileNumber = "01784567890"
                    }
                });

            Assert.True(res.IsSuccess);
            Assert.True(res.Data.Data > 0);
        }

        [Fact]
        public async Task POST_Create_Fail_400()
        {
            var res = await _rext.PostJSON<Result<int>>("app/create",
                new User
                {
                    Name = "Marko",
                    DateOfBirth = DateOnly.Parse("1991-03-12"),
                    Contact = new()
                    {
                        EmailAddress = "orlando.b@example.com",
                        MobileNumber = "01784567890"
                    }
                });

            Assert.False(res.IsSuccess);
            Assert.False(res.Data?.Data > 0);
        }
    }
}
