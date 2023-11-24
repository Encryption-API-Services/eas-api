using API.Controllers;
using API.ControllersLogic;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.UserAuthentication;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;
using Xunit.Priority;

namespace Controllers.Tests
{
    public class PasswordControllerTests
    {
        private readonly PasswordController _passwordController;
        private readonly IUserRepository _userRepository;
        public PasswordControllerTests()
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
            string token = new AuthenticationHelper().GetStandardTestFrameworkToken().GetAwaiter().GetResult();
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Request.Headers["Authorization"]).Returns(String.Format("Bearer {0}", token));

            this._passwordController = new PasswordController(new PasswordControllerLogic(
                    new HashedPasswordRepository(databaseSettings, client),
                    new UserRepository(databaseSettings, client),
                    new ForgotPasswordRepository(databaseSettings, client),
                    new EASExceptionRepository(databaseSettings, client),
                    new BenchmarkMethodCache(databaseSettings, client)
                ), mockHttpContextAccessor.Object);

            this._userRepository = new UserRepository(databaseSettings, client);
        }


        [Fact]
        public async Task BCryptEncrypt()
        {
            string passwordToHash = "SomeReallyBadPassword";
            OkObjectResult result = await this._passwordController.BcryptPassword(new BCryptEncryptModel() { Password = passwordToHash }) as OkObjectResult;
            BCryptEncryptResponseModel test = JsonConvert.DeserializeObject<BCryptEncryptResponseModel>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(test.HashedPassword);
        }

        [Fact]
        public async Task BCryptIsValid()
        {
            string passwordToHash = "DoNotUseThisPassword";
            OkObjectResult hashResult = await this._passwordController.BcryptPassword(new BCryptEncryptModel() { Password = passwordToHash }) as OkObjectResult;
            BCryptEncryptResponseModel encryptionResult = JsonConvert.DeserializeObject<BCryptEncryptResponseModel>(JsonConvert.SerializeObject(hashResult.Value));
            OkObjectResult isValidResult = await this._passwordController.BcryptVerifyPassword(new BcryptVerifyModel() { HashedPassword = encryptionResult.HashedPassword, Password = passwordToHash }) as OkObjectResult;
            BCryptVerifyResponseModel isValid = JsonConvert.DeserializeObject<BCryptVerifyResponseModel>(JsonConvert.SerializeObject(isValidResult.Value));
            Assert.Equal(true, isValid.IsValid);
            Assert.Equal(200, isValidResult.StatusCode);
        }

        [Fact]
        public async Task SCryptEncrypt()
        {
            string passwordToHash = "Scrypt";
            OkObjectResult hashResult = await this._passwordController.SCryptEncrypt(new ScryptHashRequest() { passwordToHash = passwordToHash }) as OkObjectResult;
            SCryptHashResponse hashResultParsed = JsonConvert.DeserializeObject<SCryptHashResponse>(JsonConvert.SerializeObject(hashResult.Value));
            Assert.NotNull(hashResultParsed.HashedPassword);
            Assert.Equal(200, hashResult.StatusCode);
        }

        [Fact]
        public async Task SCryptVerify()
        {
            string passwordToHash = "ScryptVerify";
            OkObjectResult hashResult = await this._passwordController.SCryptEncrypt(new ScryptHashRequest() { passwordToHash = passwordToHash }) as OkObjectResult;
            SCryptHashResponse hashResultParsed = JsonConvert.DeserializeObject<SCryptHashResponse>(JsonConvert.SerializeObject(hashResult.Value));
            OkObjectResult verifyResult = await this._passwordController.SCryptVerify(new SCryptVerifyRequest() { hashedPassword = hashResultParsed.HashedPassword, password = passwordToHash }) as OkObjectResult;
            SCryptVerifyResponse hashVerify = JsonConvert.DeserializeObject<SCryptVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(200, verifyResult.StatusCode);
            Assert.Equal(true, hashVerify.IsValid);
        }

        [Fact]
        public async Task Argon2Encrypt()
        {
            string passwordToHash = "ScryptVerify";
            OkObjectResult hashResult = await this._passwordController.Argon2Hash(new Argon2HashRequest() { passwordToHash = passwordToHash }) as OkObjectResult;
            Argon2HashResponse hashResponse = JsonConvert.DeserializeObject<Argon2HashResponse>(JsonConvert.SerializeObject(hashResult.Value));
            Assert.Equal(200, hashResult.StatusCode);
            Assert.NotNull(hashResponse.HashedPassword);
        }

        [Fact]
        public async Task Argon2Verify()
        {
            string passwordToHash = "ScryptVerify/Argon2";
            OkObjectResult hashResult = await this._passwordController.Argon2Hash(new Argon2HashRequest() { passwordToHash = passwordToHash }) as OkObjectResult;
            Argon2HashResponse hashResponse = JsonConvert.DeserializeObject<Argon2HashResponse>(JsonConvert.SerializeObject(hashResult.Value));
            OkObjectResult verifyResult = await this._passwordController.Argon2Verify(new Argon2VerifyRequest() { hashedPassword = hashResponse.HashedPassword, password = passwordToHash }) as OkObjectResult;
            Argon2VerifyResponse verifyResponse = JsonConvert.DeserializeObject<Argon2VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(200, verifyResult.StatusCode);
            Assert.Equal(true, verifyResponse.IsValid);
        }

        [Fact, Priority(-10)]
        public async Task ForgotPassword()
        {
            ForgotPasswordRequest request = new ForgotPasswordRequest() { Email = "mikemulchrone987@gmail.com" };
            OkObjectResult result = await this._passwordController.ForgotPassword(request) as OkObjectResult;
            Assert.Equal(200, result.StatusCode);
        }
    }
}