using API.ControllerLogic;
using API.Controllers;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.ED25519;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;

namespace Controllers.Tests
{
    public class ED25519ControllerTests
    {
        private readonly ED25519Controller _ed25519Controller;
        public ED25519ControllerTests()
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

            this._ed25519Controller = new ED25519Controller(new ED25519ControllerLogic(
                new EASExceptionRepository(databaseSettings, client),
                new BenchmarkMethodCache(databaseSettings, client)),
                mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetED25519DalekKeyPair()
        {
            OkObjectResult keyPair = await this._ed25519Controller.GetED25519KeyPair() as OkObjectResult;
            ED25519GetKeyPairResponse response = JsonConvert.DeserializeObject<ED25519GetKeyPairResponse>(JsonConvert.SerializeObject(keyPair.Value));
            Assert.Equal(keyPair.StatusCode, 200);
            Assert.NotNull(response.KeyPair);
            Assert.True(response.KeyPair.Length > 0);
        }

        [Fact]
        public async Task ED25519Sign()
        {
            OkObjectResult keyPair = await this._ed25519Controller.GetED25519KeyPair() as OkObjectResult;
            ED25519GetKeyPairResponse response = JsonConvert.DeserializeObject<ED25519GetKeyPairResponse>(JsonConvert.SerializeObject(keyPair.Value));
            ED25519SignWithKeyPairRequest request = new ED25519SignWithKeyPairRequest()
            {
                KeyPair = response.KeyPair,
                DataToSign = "Hello World"
            };
            OkObjectResult result = await this._ed25519Controller.SignWithKeyPair(request) as OkObjectResult;
            ED25519SignDataResponse signResponse = JsonConvert.DeserializeObject<ED25519SignDataResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(signResponse.Signature);
            Assert.True(signResponse.Signature.Length > 0);
            Assert.NotNull(signResponse.PublicKey);
            Assert.True(signResponse.PublicKey.Length > 0);
        }

        [Fact]
        public async Task Ed25519VerifyPass()
        {
            OkObjectResult keyPair = await this._ed25519Controller.GetED25519KeyPair() as OkObjectResult;
            ED25519GetKeyPairResponse response = JsonConvert.DeserializeObject<ED25519GetKeyPairResponse>(JsonConvert.SerializeObject(keyPair.Value));
            ED25519SignWithKeyPairRequest request = new ED25519SignWithKeyPairRequest()
            {
                KeyPair = response.KeyPair,
                DataToSign = "Hello World"
            };
            OkObjectResult result = await this._ed25519Controller.SignWithKeyPair(request) as OkObjectResult;
            ED25519SignDataResponse signResponse = JsonConvert.DeserializeObject<ED25519SignDataResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult verifyResult = await this._ed25519Controller.VerifyWithPublicKey(new Ed25519VerifyWithPublicKeyRequest()
            {
                DataToVerify = "Hello World",
                PublicKey = signResponse.PublicKey,
                Signature = signResponse.Signature
            }) as OkObjectResult;
            ED25519VerifyResponse verifyResponse = JsonConvert.DeserializeObject<ED25519VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.True(verifyResponse.IsValid);
        }

        [Fact]
        public async Task Ed25519VerifyFail()
        {
            OkObjectResult keyPair = await this._ed25519Controller.GetED25519KeyPair() as OkObjectResult;
            ED25519GetKeyPairResponse response = JsonConvert.DeserializeObject<ED25519GetKeyPairResponse>(JsonConvert.SerializeObject(keyPair.Value));
            ED25519SignWithKeyPairRequest request = new ED25519SignWithKeyPairRequest()
            {
                KeyPair = response.KeyPair,
                DataToSign = "Hello World"
            };
            OkObjectResult result = await this._ed25519Controller.SignWithKeyPair(request) as OkObjectResult;
            ED25519SignDataResponse signResponse = JsonConvert.DeserializeObject<ED25519SignDataResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult verifyResult = await this._ed25519Controller.VerifyWithPublicKey(new Ed25519VerifyWithPublicKeyRequest()
            {
                DataToVerify = "Hello World123",
                PublicKey = signResponse.PublicKey,
                Signature = signResponse.Signature
            }) as OkObjectResult;
            ED25519VerifyResponse verifyResponse = JsonConvert.DeserializeObject<ED25519VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.True(!verifyResponse.IsValid);
        }
    }
}
