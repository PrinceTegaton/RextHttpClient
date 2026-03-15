using Rext.ApiSimulator;
using System.Net;
using System.Text;

namespace Rext.UnitTests
{
    public class ClientJsonAndStringTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        private IRextHttpClient _rext;

        public ClientJsonAndStringTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7173/api/app");

            _rext = new RextHttpClient(
                configuration: new()
                {
                    ThrowExceptionIfNotSuccessResponse = false,
                    ThrowExceptionOnDeserializationFailure = false,
                    ReadResponseHeaders = true,
                },
                httpClient: _client);
        }

        #region GET
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GET_ByQuery(int id)
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
        public async Task GET_ByPath(int id)
        {
            var res = await _rext.GetJSON<Result<User>>($"getbypath/{id}");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Fact]
        public async Task GET_ByPath_Fail_400()
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
        #endregion

        #region POST
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
        #endregion

        #region PATCH/PUT/DELETE
        [Fact]
        public async Task PATCH_Update()
        {
            var res = await _rext.PatchJSON<Result<User>>("updatepartial?id=1&newEmail=markok9@xmail.go");
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data.Data);
            Assert.Equal("markok9@xmail.go", res.Data.Data.Contact?.EmailAddress);
        }

        [Fact]
        public async Task PUT_Update()
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

        [Theory]
        [InlineData(6, "Just for fun")]
        [InlineData(8, "Just for fun")]
        [InlineData(9, "Just for fun")]
        public async Task DELETE_WithQuery(int id, string note)
        {
            var res = await _rext.Delete($"delete?id={id}&note={note}");
            Assert.True(res.IsSuccess);
        }
        #endregion

        #region HEADERS
        [Theory]
        [InlineData(1, "Record with id 1")]
        [InlineData(2, "Next record with id 2")]
        [InlineData(3, "Just another ecord with id 3")]
        public async Task Header_Input_ByExtensionMethodSingle(int id, string headerText)
        {
            var res = await _rext.AddHeader("id", id.ToString())
                                 .AddHeader("fancyHeader", headerText)
                                 .GetJSON<Result<User>>("getbyheaderinput");

            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.Equal(res.Data.Message, headerText);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Theory]
        [InlineData(8, "Record with id 8")]
        [InlineData(9, "Next record with id 9")]
        [InlineData(10, "Just another record with id 10")]
        public async Task Header_Input_ByExtensionMethodCollectionBulk(int id, string headerText)
        {
            Dictionary<string, string> header = new() {
                { "id", id.ToString() },
                { "fancyHeader", headerText }
            };

            var res = await _rext.AddHeader(header)
                                 .GetJSON<Result<User>>("getbyheaderinput");

            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.Equal(res.Data.Message, headerText);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Theory]
        [InlineData(8, "Record with id 8")]
        [InlineData(9, "Next record with id 9")]
        [InlineData(10, "Just another ecord with id 10")]
        public async Task Header_Input_ByCollectionSingle(int id, string headerText)
        {
            _rext.Headers.TryAdd("id", id.ToString());
            _rext.Headers.TryAdd("fancyHeader", headerText);

            var res = await _rext.GetJSON<Result<User>>("getbyheaderinput");

            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotNull(res.Data.Data);
            Assert.Equal(res.Data.Data.Id, id);
            Assert.Equal(res.Data.Message, headerText);
            Assert.NotEmpty(res.Data.Data.Name);
        }

        [Theory]
        [InlineData("XYZ0902897621===")]
        [InlineData("Tytuj2mHJSDYuj.dhjwhjn29178==")]
        public async Task Header_BearerAuth(string token)
        {
            _rext.UseBearerAuthentication(token);

            bool hasAuth = _rext.Headers.TryGetValue("Authorization", out var authHeader);

            Assert.True(hasAuth);
            Assert.StartsWith("Bearer", authHeader, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(token, authHeader?.Split(' ')[1]);
        }

        [Theory]
        [InlineData("user.123", "w_xx123")]
        [InlineData("user.xyz", "Pxx123")]
        public async Task Header_BasicAuth(string username, string password)
        {
            _rext.UseBasicAuthentication(username, password);

            bool hasAuth = _rext.Headers.TryGetValue("Authorization", out var authHeader);

            Assert.True(hasAuth);
            Assert.StartsWith("Basic", authHeader, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")), authHeader?.Split(' ')[1]);
        }

        [Theory]
        [InlineData(100, "Record X")]
        [InlineData(200, "Pretty Header")]
        public async Task Header_Output(int id, string headerText)
        {
            var res = await _rext.GetJSON<Result>($"getbyheaderoutput?id={id}&fancyHeader={headerText}");
            
            Assert.NotNull(res.Headers);
            Assert.NotEmpty(res.Headers);
            Assert.Equal(id.ToString(), res.Headers["r-id"]);
            Assert.Equal(headerText, res.Headers["r-fancyHeader"]);
        }

        #endregion

        // todo: form-content, global json serializer, beforeCall, afterCall, resiliency
    }
}
