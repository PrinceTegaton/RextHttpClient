using Microsoft.AspNetCore.Mvc.Testing;
using Rext.ApiSimulator;
using System.Net;

namespace Rext.UnitTests
{
    public class ClientTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        private IRextHttpClient _rext;

        public ClientTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7173/api/app");

            _rext = new RextHttpClient(
                configuration: new()
                {
                    ThrowExceptionIfNotSuccessResponse = false,
                    ThrowExceptionOnDeserializationFailure = false
                },
                httpClient: _client);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GET_WithQuery(int id)
        {
            var res = await _rext.GetJSON<Result<User>>($"getbyquery?id={id}");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Fact]
        public async Task GET_WithQuery_Fail_400()
        {
            var res = await _rext.GetJSON<Result<User>>("getbyquery?id=100");
            Assert.False(res.IsSuccess);
            Assert.Null(res.Data);
            Assert.Equal(res?.StatusCode, HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public async Task GET_WithPath(int id)
        {
            var res = await _rext.GetJSON<Result<User>>($"getbypath/{id}");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Fact]
        public async Task GET_WithPath_Fail_400()
        {
            var res = await _rext.GetJSON<Result<User>>("getbypath/150");
            Assert.False(res.IsSuccess);
            Assert.Null(res.Data);
            Assert.Equal(res?.StatusCode, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_PlainString()
        {
            var res = await _rext.GetString("getbyquery?id=1");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.StartsWith("{", res.Data);
            Assert.EndsWith("}", res.Data);
        }

        [Fact]
        public async Task GET_404()
        {
            var res = await _rext.GetString("getbyqueryxx");
            Assert.False(res.IsSuccess);
            Assert.Equal(res?.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_List()
        {
            var res = await _rext.GetJSON<Result<IEnumerable<User>>>("getall");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data);
        }

        [Fact]
        public async Task GET_List_WithQuery_WithWrapper()
        {
            var res = await _rext.GetJSON<Result<IEnumerable<User>>>("getallwithquery?filter_name=carlos&withwrapper=true");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.NotEmpty(res.Data.Data);
        }

        [Fact]
        public async Task GET_List_WithQuery_WithoutWrapper()
        {
            var res = await _rext.GetJSON<IEnumerable<User>>("getallwithquery?filter_name=carlos&withwrapper=false");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotEmpty(res.Data);
        }

        [Fact]
        public async Task POST_Create()
        {
            var res = await _rext.PostJSON<Result<int>>("create",
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
        public async Task POST_404()
        {
            var res = await _rext.PostJSON<Result<int>>("createxx", null);

            Assert.False(res.IsSuccess);
            Assert.Equal(res?.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task POST_Create_Fail_400()
        {
            var res = await _rext.PostJSON<Result<int>>("create", null);

            Assert.False(res.IsSuccess);
            Assert.False(res.Data?.Data > 0);
        }

        [Fact]
        public async Task PATCH_update()
        {
            var res = await _rext.PatchJSON<Result<User>>("updatepartial?id=1&newEmail=markok9@xmail.go");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data.Data);
            Assert.Equal("markok9@xmail.go", res.Data.Data.Contact?.EmailAddress);
        }

        [Fact]
        public async Task PUT_update()
        {
            var res = await _rext.PutJSON<Result<User>>("update",
                new User
                {
                    Id = 5,
                    Name = "Ethan Kaluya",
                    DateOfBirth = DateOnly.Parse("1996-06-16"),
                    Contact = new()
                    {
                        EmailAddress = "k.ethan@example22.com",
                        MobileNumber = "02865567820"
                    }
                });

            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data.Data);
            Assert.Equal("Ethan Kaluya", res.Data.Data.Name);
            Assert.Equal(DateOnly.Parse("1996-06-16"), res.Data.Data.DateOfBirth);
            Assert.Equal("k.ethan@example22.com", res.Data.Data.Contact?.EmailAddress);
            Assert.Equal("02865567820", res.Data.Data.Contact?.MobileNumber);
        }

        [Fact]
        public async Task MethodNotAllowed_405()
        {
            var res = await _rext.GetString("create");
            Assert.False(res.IsSuccess);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, res.StatusCode);
        }
    }
}
