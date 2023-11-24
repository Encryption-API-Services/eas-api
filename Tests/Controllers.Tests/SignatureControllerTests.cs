using API.ControllerLogic;
using API.Controllers;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.Signatures;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;

namespace Controllers.Tests
{
    public class SignatureControllerTests
    {
        private readonly SignatureController _signatureController;
        public SignatureControllerTests()
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

            this._signatureController = new SignatureController(
                new SignatureControllerLogic(
                    new EASExceptionRepository(databaseSettings, client),
                    new BenchmarkMethodCache(databaseSettings, client)
               ), mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task HMACSignTest()
        {
            string key = "HmacSigningKey";
            string dataToSign = "SomeReallyGoodDataToSign";
            OkObjectResult result = await this._signatureController.HMACSign(new HMACSignRequest() { Key = key, Message = dataToSign }) as OkObjectResult;
            HMACSignResponse response = JsonConvert.DeserializeObject<HMACSignResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(response.Signature);
        }

        [Fact]
        public async Task HMACVerifyTest()
        {
            string key = "SigningKey";
            string message = "MessageToSign";
            OkObjectResult result = await this._signatureController.HMACSign(new HMACSignRequest() { Key = key, Message = message }) as OkObjectResult;
            HMACSignResponse response = JsonConvert.DeserializeObject<HMACSignResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult verifyResult = await this._signatureController.HMACVerify(new HMACVerifyRequest() { Key = key, Message = message, Signature = response.Signature }) as OkObjectResult;
            HMACVerifyResponse response2 = JsonConvert.DeserializeObject<HMACVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.Equal(response2.IsValid, true);
        }

        [Fact]
        public async Task SHA512ED25519SignTest()
        {
            string dataToSign = "SignThisData";
            OkObjectResult result = await this._signatureController.SHA512ED25519DalekSign(new SHA512ED25519DalekSignRequest() { DataToSign = dataToSign }) as OkObjectResult;
            SHA512ED25519DalekSignResponse response = JsonConvert.DeserializeObject<SHA512ED25519DalekSignResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.Signature);
        }

        [Fact]
        public async Task SHA512ED25519VerifyTest()
        {
            string dataToSign = "SignThisData";
            OkObjectResult result = await this._signatureController.SHA512ED25519DalekSign(new SHA512ED25519DalekSignRequest() { DataToSign = dataToSign }) as OkObjectResult;
            SHA512ED25519DalekSignResponse response = JsonConvert.DeserializeObject<SHA512ED25519DalekSignResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult verifyResult = await this._signatureController.SHA512ED25519DalekVerify(new SHA512ED25519DalekVerifyRequest() { Signature = response.Signature, DataToVerify = dataToSign, PublicKey = response.PublicKey }) as OkObjectResult;
            SHA512ED25519DalekVerifyResponse verifyResponse = JsonConvert.DeserializeObject<SHA512ED25519DalekVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.Equal(verifyResponse.IsValid, true);
        }

        [Fact]
        public async Task SHA512RsaSignTest()
        {
            string dataToHash = "DataToHash";
            int keySize = 2048;
            OkObjectResult result = await this._signatureController.SHA512SignedRSA(new SHA512SignedRSARequest() { DataToHash = dataToHash, KeySize = keySize }) as OkObjectResult;
            SHA512SignedRSAResponse response = JsonConvert.DeserializeObject<SHA512SignedRSAResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.PrivateKey);
            Assert.NotNull(response.Signature);
        }

        [Fact]
        public async Task SHA512RsaVerifyTest()
        {
            string dataToHash = "DataToHash";
            int keySize = 2048;
            OkObjectResult result = await this._signatureController.SHA512SignedRSA(new SHA512SignedRSARequest() { DataToHash = dataToHash, KeySize = keySize }) as OkObjectResult;
            SHA512SignedRSAResponse response = JsonConvert.DeserializeObject<SHA512SignedRSAResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult verifyResult = await this._signatureController.SHA512SignedRSAVerify(new SHA512SignedRSAVerifyRequest() { PublicKey = response.PublicKey, Signature = response.Signature, OriginalData = dataToHash }) as OkObjectResult;
            SHA512SignedRSAVerifyResponse verifyResponse = JsonConvert.DeserializeObject<SHA512SignedRSAVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.Equal(verifyResponse.IsValid, true);
        }

        [Fact]
        public async Task Blake2RsaSignTest()
        {
            Blake2RSASignRequest request = new Blake2RSASignRequest()
            {
                RsaKeySize = 4096,
                Blake2HashSize = 512,
                DataToSign = "DataToHash"
            };
            OkObjectResult result = await this._signatureController.Blake2RsaSign(request) as OkObjectResult;
            Blake2RSASignResponse response = JsonConvert.DeserializeObject<Blake2RSASignResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.PrivateKey);
            Assert.NotNull(response.Signature);
            Assert.NotEqual(response.Signature, request.DataToSign);
        }

        [Fact]
        public async Task Blake2RsaVerifyPassTest()
        {
            Blake2RSASignRequest request = new Blake2RSASignRequest()
            {
                RsaKeySize = 4096,
                Blake2HashSize = 512,
                DataToSign = "DataToHash"
            };
            OkObjectResult result = await this._signatureController.Blake2RsaSign(request) as OkObjectResult;
            Blake2RSASignResponse response = JsonConvert.DeserializeObject<Blake2RSASignResponse>(JsonConvert.SerializeObject(result.Value));
            Blake2RSAVerifyRequest verifyRequest = new Blake2RSAVerifyRequest()
            {
                PublicKey = response.PublicKey,
                Signature = response.Signature,
                OriginalData = "DataToHash",
                Blake2HashSize = 512
            };
            OkObjectResult verifyResult = await this._signatureController.Blake2RsaVerify(verifyRequest) as OkObjectResult;
            Blake2RSAVerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2RSAVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.True(verifyResponse.IsValid);
        }

        [Fact]
        public async Task Blake2RsaVerifyFailTest()
        {
            Blake2RSASignRequest request = new Blake2RSASignRequest()
            {
                RsaKeySize = 4096,
                Blake2HashSize = 256,
                DataToSign = "HashThisStuffBase"
            };
            OkObjectResult result = await this._signatureController.Blake2RsaSign(request) as OkObjectResult;
            Blake2RSASignResponse response = JsonConvert.DeserializeObject<Blake2RSASignResponse>(JsonConvert.SerializeObject(result.Value));
            Blake2RSAVerifyRequest verifyRequest = new Blake2RSAVerifyRequest()
            {
                PublicKey = response.PublicKey,
                Signature = response.Signature,
                OriginalData = "DataToHash9821564123546",
                Blake2HashSize = 256
            };
            OkObjectResult verifyResult = await this._signatureController.Blake2RsaVerify(verifyRequest) as OkObjectResult;
            Blake2RSAVerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2RSAVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.True(!verifyResponse.IsValid);
        }

        [Fact]
        public async Task Blake2ED25519DalekSignTest()
        {
            Blake2ED25519DalekSignRequest request = new Blake2ED25519DalekSignRequest() { DataToSign = "DataToSign", HashSize = 256 };
            OkObjectResult result = await this._signatureController.Blake2ED25519DalekSign(request) as OkObjectResult;
            Blake2ED25519DalekSignResponse response = JsonConvert.DeserializeObject<Blake2ED25519DalekSignResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(result.StatusCode, 200);
            Assert.NotNull(response.PublicKey);
            Assert.NotNull(response.Signature);
            Assert.NotEqual(response.Signature, request.DataToSign);
        }

        [Fact]
        public async Task Blake2ED25519DalekVerifyPass()
        {
            Blake2ED25519DalekSignRequest request = new Blake2ED25519DalekSignRequest() { DataToSign = "DataToSign", HashSize = 512 };
            OkObjectResult result = await this._signatureController.Blake2ED25519DalekSign(request) as OkObjectResult;
            Blake2ED25519DalekSignResponse response = JsonConvert.DeserializeObject<Blake2ED25519DalekSignResponse>(JsonConvert.SerializeObject(result.Value));
            Blake2ED25519DalekVerifyRequest verifyRequest = new Blake2ED25519DalekVerifyRequest() { PublicKey = response.PublicKey, Signature = response.Signature, DataToVerify = "DataToSign", HashSize = 512 };
            OkObjectResult verifyResult = await this._signatureController.Blake2ED25519DalekVerify(verifyRequest) as OkObjectResult;
            Blake2ED25519DalekVerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2ED25519DalekVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.True(verifyResponse.IsValid);
            Assert.Equal(verifyResult.StatusCode, 200);
        }

        [Fact]
        public async Task Blake2ED25519DalekVerifyFail()
        {
            Blake2ED25519DalekSignRequest request = new Blake2ED25519DalekSignRequest() { DataToSign = "DataToSign", HashSize = 256 };
            OkObjectResult result = await this._signatureController.Blake2ED25519DalekSign(request) as OkObjectResult;
            Blake2ED25519DalekSignResponse response = JsonConvert.DeserializeObject<Blake2ED25519DalekSignResponse>(JsonConvert.SerializeObject(result.Value));
            Blake2ED25519DalekVerifyRequest verifyRequest = new Blake2ED25519DalekVerifyRequest() { PublicKey = response.PublicKey, Signature = response.Signature, DataToVerify = "DataToSign123456", HashSize = 256 };
            OkObjectResult verifyResult = await this._signatureController.Blake2ED25519DalekVerify(verifyRequest) as OkObjectResult;
            Blake2ED25519DalekVerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2ED25519DalekVerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(verifyResult.StatusCode, 200);
            Assert.True(!verifyResponse.IsValid);
        }
    }
}
