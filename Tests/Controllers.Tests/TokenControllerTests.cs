using API.ControllerLogic;
using API.Controllers;
using CASHelpers;
using CASHelpers.Types.HttpResponses.UserAuthentication;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;

namespace Controllers.Tests
{
    public class TokenControllerTests
    {
        private readonly TokenController _tokenController;

        public TokenControllerTests()
        {
            IDatabaseSettings databaseSettings = new DatabaseSettings
            {
                Connection = Environment.GetEnvironmentVariable("Connection"),
                UserCollectionName = Environment.GetEnvironmentVariable("UserCollectionName"),
                DatabaseName = Environment.GetEnvironmentVariable("DatabaseName")
            };

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
            var client = new MongoClient(settings);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var http = new Mock<HttpContext>(MockBehavior.Loose);
            var request = new Mock<HttpRequest>(MockBehavior.Loose);

            mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(http.Object);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request).Returns(request.Object);
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            string token = authenticationHelper.GetExpiredTestFrameworkToken().GetAwaiter().GetResult();
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request.Headers[Constants.HeaderNames.Authorization]).Returns(String.Format("Bearer {0}", token));
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Items[Constants.HttpItems.UserID]).Returns(new JWT().GetUserIdFromToken(token));
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request.Headers[Constants.HeaderNames.ApiKey]).Returns(Environment.GetEnvironmentVariable("EasApiKey"));
            this._tokenController = new TokenController(mockHttpContextAccessor.Object, new TokenControllerLogic(
                new UserRepository(databaseSettings, client),
                new EASExceptionRepository(databaseSettings, client),
                new DataLayer.Cache.BenchmarkMethodCache(databaseSettings, client)));
        }

        [Fact]
        public async Task GetToken()
        {
            OkObjectResult tokenResult = await this._tokenController.GetToken() as OkObjectResult;
            GetTokenResponse tokenResponse = JsonConvert.DeserializeObject<GetTokenResponse>(JsonConvert.SerializeObject(tokenResult.Value));
            Assert.Equal(200, tokenResult.StatusCode);
            Assert.NotNull(tokenResponse.Token);
        }

        [Fact]
        public async Task GetRefreshToken()
        {
            OkObjectResult tokenResult = await this._tokenController.GetRefreshToken() as OkObjectResult;
            GetTokenResponse tokenResponse = JsonConvert.DeserializeObject<GetTokenResponse>(JsonConvert.SerializeObject(tokenResult.Value));
            Assert.Equal(200, tokenResult.StatusCode);
            Assert.NotNull(tokenResponse.Token);
        }
    }
}
