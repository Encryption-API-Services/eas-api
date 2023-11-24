using API.ControllerLogic;
using API.Controllers;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Encryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;

namespace Controllers.Tests
{
    public class RsaControllerTests
    {
        private readonly RsaController _rsaController;
        public RsaControllerTests()
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
            mockHttpContextAccessor.SetupGet(x => x.HttpContext.Items["UserID"]).Returns(new JWT().GetUserIdFromToken(token));



            this._rsaController = new RsaController(mockHttpContextAccessor.Object, new RsaControllerLogic(
                new EASExceptionRepository(databaseSettings, client),
                new RsaEncryptionRepository(databaseSettings, client),
                new BenchmarkMethodCache(databaseSettings, client)));
        }

        [Fact]
        public async Task GetRsaKeyPairTest()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            Assert.True(keyPairResult.StatusCode == 200);
            Assert.NotNull(rsaGetKeyPairResponse.PublicKey);
            Assert.NotNull(rsaGetKeyPairResponse.PrivateKey);
        }

        [Fact]
        public async Task RsaSignWithKey()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            RsaSignWithKeyRequest rsaSignWithKeyRequest = new RsaSignWithKeyRequest
            {
                PrivateKey = rsaGetKeyPairResponse.PrivateKey,
                DataToSign = "test"
            };
            OkObjectResult signResult = await this._rsaController.SignWithKey(rsaSignWithKeyRequest) as OkObjectResult;
            RsaSignWithKeyResponse rsaSignWithKeyResponse = JsonConvert.DeserializeObject<RsaSignWithKeyResponse>(JsonConvert.SerializeObject(signResult.Value));
            Assert.NotNull(rsaSignWithKeyResponse.Signature);
        }

        [Fact]
        public async Task RsaVerifyPass()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            RsaSignWithKeyRequest rsaSignWithKeyRequest = new RsaSignWithKeyRequest
            {
                PrivateKey = rsaGetKeyPairResponse.PrivateKey,
                DataToSign = "test"
            };
            OkObjectResult signResult = await this._rsaController.SignWithKey(rsaSignWithKeyRequest) as OkObjectResult;
            RsaSignWithKeyResponse rsaSignWithKeyResponse = JsonConvert.DeserializeObject<RsaSignWithKeyResponse>(JsonConvert.SerializeObject(signResult.Value));
            RsaVerifyRequest rsaVerifyRequest = new RsaVerifyRequest
            {
                PublicKey = rsaGetKeyPairResponse.PublicKey,
                OriginalData = "test",
                Signature = rsaSignWithKeyResponse.Signature
            };
            OkObjectResult rsaVerifyResult = await this._rsaController.Verify(rsaVerifyRequest) as OkObjectResult;
            RsaVerifyResponse verifyResponse = JsonConvert.DeserializeObject<RsaVerifyResponse>(JsonConvert.SerializeObject(rsaVerifyResult.Value));
            Assert.True(verifyResponse.IsValid == true);
        }

        [Fact]
        public async Task RsaVerifyFail()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            RsaSignWithKeyRequest rsaSignWithKeyRequest = new RsaSignWithKeyRequest
            {
                PrivateKey = rsaGetKeyPairResponse.PrivateKey,
                DataToSign = "test"
            };
            OkObjectResult signResult = await this._rsaController.SignWithKey(rsaSignWithKeyRequest) as OkObjectResult;
            RsaSignWithKeyResponse rsaSignWithKeyResponse = JsonConvert.DeserializeObject<RsaSignWithKeyResponse>(JsonConvert.SerializeObject(signResult.Value));
            RsaVerifyRequest rsaVerifyRequest = new RsaVerifyRequest
            {
                PublicKey = rsaGetKeyPairResponse.PublicKey,
                OriginalData = "test123",
                Signature = rsaSignWithKeyResponse.Signature
            };
            OkObjectResult rsaVerifyResult = await this._rsaController.Verify(rsaVerifyRequest) as OkObjectResult;
            RsaVerifyResponse verifyResponse = JsonConvert.DeserializeObject<RsaVerifyResponse>(JsonConvert.SerializeObject(rsaVerifyResult.Value));
            Assert.True(verifyResponse.IsValid == false);
        }

        [Fact]
        public async Task RsaEncryptWithKey()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            EncryptWithPublicRequest rsaEncryptWithKeyRequest = new EncryptWithPublicRequest
            {
                PublicKey = rsaGetKeyPairResponse.PublicKey,
                DataToEncrypt = "test"
            };
            OkObjectResult encryptResult = await this._rsaController.EncryptWithPublic(rsaEncryptWithKeyRequest) as OkObjectResult;
            EncryptWithPublicResponse rsaEncryptWithKeyResponse = JsonConvert.DeserializeObject<EncryptWithPublicResponse>(JsonConvert.SerializeObject(encryptResult.Value));
            Assert.NotNull(rsaEncryptWithKeyResponse.EncryptedData);
            Assert.NotEqual(rsaEncryptWithKeyResponse.EncryptedData, "test");
        }

        [Fact]
        public async Task RsaDecrypt()
        {
            int keySize = 2048;
            OkObjectResult keyPairResult = await this._rsaController.GetKeyPair(keySize) as OkObjectResult;
            RsaGetKeyPairResponse rsaGetKeyPairResponse = JsonConvert.DeserializeObject<RsaGetKeyPairResponse>(JsonConvert.SerializeObject(keyPairResult.Value));
            EncryptWithPublicRequest rsaEncryptWithKeyRequest = new EncryptWithPublicRequest
            {
                PublicKey = rsaGetKeyPairResponse.PublicKey,
                DataToEncrypt = "test"
            };
            OkObjectResult encryptResult = await this._rsaController.EncryptWithPublic(rsaEncryptWithKeyRequest) as OkObjectResult;
            EncryptWithPublicResponse rsaEncryptWithKeyResponse = JsonConvert.DeserializeObject<EncryptWithPublicResponse>(JsonConvert.SerializeObject(encryptResult.Value));
            RsaDecryptRequest request = new RsaDecryptRequest()
            {
                PrivateKey = rsaGetKeyPairResponse.PrivateKey,
                DataToDecrypt = rsaEncryptWithKeyResponse.EncryptedData
            };
            OkObjectResult decryptResult = await this._rsaController.Decrypt(request) as OkObjectResult;
            RsaDecryptResponse rsaDecryptResponse = JsonConvert.DeserializeObject<RsaDecryptResponse>(JsonConvert.SerializeObject(decryptResult.Value));
            Assert.Equal(rsaDecryptResponse.DecryptedData, rsaEncryptWithKeyRequest.DataToEncrypt);
        }

        [Fact]
        public async Task SignWithoutKey()
        {
            RsaSignWithoutKeyRequest request = new RsaSignWithoutKeyRequest()
            {
                dataToSign = "test",
                keySize = 1024
            };
            OkObjectResult result = await this._rsaController.SignWithoutKey(request) as OkObjectResult;
            RsaSignWithoutKeyResponse response = JsonConvert.DeserializeObject<RsaSignWithoutKeyResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.Signature);
        }

        [Fact]
        public async Task EncryptWithoutPublic()
        {
            RsaEncryptWithoutPublicRequest request = new RsaEncryptWithoutPublicRequest()
            {
                dataToEncrypt = "test",
                keySize = 2048
            };
            OkObjectResult result = await this._rsaController.EncryptWithoutPublic(request) as OkObjectResult;
            RsaEncryptWithoutPublicResponse response = JsonConvert.DeserializeObject<RsaEncryptWithoutPublicResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.EncryptedData);
            Assert.NotEqual(response.EncryptedData, request.dataToEncrypt);
        }

        [Fact]
        public async Task DecryptWithStoredPrivate()
        {
            RsaEncryptWithoutPublicRequest request = new RsaEncryptWithoutPublicRequest()
            {
                dataToEncrypt = "test",
                keySize = 2048
            };
            OkObjectResult result = await this._rsaController.EncryptWithoutPublic(request) as OkObjectResult;
            RsaEncryptWithoutPublicResponse response = JsonConvert.DeserializeObject<RsaEncryptWithoutPublicResponse>(JsonConvert.SerializeObject(result.Value));
            RsaDecryptWithStoredPrivateRequest decryptRequest = new RsaDecryptWithStoredPrivateRequest()
            {
                DataToDecrypt = response.EncryptedData,
                PublicKey = response.PublicKey
            };
            OkObjectResult decryptResult = await this._rsaController.DecryptWithStoredPrivate(decryptRequest) as OkObjectResult;
            RsaDecryptWithStoredPrivateResponse decryptResponse = JsonConvert.DeserializeObject<RsaDecryptWithStoredPrivateResponse>(JsonConvert.SerializeObject(decryptResult.Value));
            Assert.Equal(200, decryptResult.StatusCode);
            Assert.NotNull(decryptResponse.DecryptedData);
            Assert.Equal(request.dataToEncrypt, decryptResponse.DecryptedData);
        }
    }
}