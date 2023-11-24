using API.Config;
using API.Controllers;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.UserAuthentication;
using MongoDB.Driver;
using Moq;
using Xunit.Priority;

namespace Controllers.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UserRegisterControllerTests
    {
        private readonly UserRegisterController _userRegisterController;
        private readonly UserRepository _userRepositry;
        public UserRegisterControllerTests()
        {
            IDatabaseSettings databaseSettings = new DatabaseSettings()
            {
                Connection = Environment.GetEnvironmentVariable("Connection"),
                DatabaseName = Environment.GetEnvironmentVariable("DatabaseName"),
                UserCollectionName = Environment.GetEnvironmentVariable("UserCollectionName")
            };

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
            var client = new MongoClient(settings);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var http = new Mock<HttpContext>(MockBehavior.Loose);
            var request = new Mock<HttpRequest>(MockBehavior.Loose);
            var headers = new Mock<IHeaderDictionary>(MockBehavior.Loose);
            var items = new Mock<IDictionary<object, object>>(MockBehavior.Loose);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(http.Object);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request).Returns(request.Object);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Items).Returns(items.Object);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request.Headers).Returns(headers.Object);
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Items["IP"]).Returns("127.0.0.1");

            this._userRegisterController = new UserRegisterController(new UserRegisterControllerLogic(
                new UserRepository(databaseSettings, client),
                new LogRequestRepository(databaseSettings, client),
                new EASExceptionRepository(databaseSettings, client),
                new BenchmarkMethodCache(databaseSettings, client)
                ), mockHttpContextAccessor.Object);

            this._userRepositry = new UserRepository(databaseSettings, client);
        }

        [Fact, Priority(-10)]
        public async Task RegisterUser()
        {
            string testUserEmail = "testuser@gmail.com";
            // Delete our test user that we are about to create.
            await this._userRepositry.DeleteUserByEmail(testUserEmail);
            RegisterUser registerUser = new RegisterUser() { email = testUserEmail, username = "testuser", password = "Mybadpassword01!@#$%" };
            OkObjectResult result = await this._userRegisterController.Post(registerUser) as OkObjectResult;
            Assert.Equal(200, result.StatusCode);
        }

        [Fact, Priority(0)]
        public async Task ActiveUser()
        {
            await Task.Delay(15000);
            User user = await this._userRepositry.GetUserByEmail("testuser@gmail.com");
            ActivateUser activeUserRequest = new ActivateUser() { Id = user.Id, Token = Base64UrlEncoder.Encode(user.EmailActivationToken.SignedToken) };
            OkObjectResult result = await this._userRegisterController.Put(activeUserRequest) as OkObjectResult;
            Assert.Equal(200, result.StatusCode);
        }
    }
}
