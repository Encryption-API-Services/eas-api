using API.Controllers;
using API.ControllersLogic;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.Encryption.AESRSAHybrid;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Tests.Helpers;

namespace Controllers.Tests
{
    public class EncryptionControllerTests
    {
        private readonly EncryptionController _encryptionController;
        public EncryptionControllerTests()
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



            this._encryptionController = new EncryptionController(new EncryptionControllerLogic(
                new BenchmarkMethodCache(databaseSettings, client),
                new EASExceptionRepository(databaseSettings, client)
                ), mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task EncryptAES()
        {
            string testString = "This is a test string";
            OkObjectResult result = await this._encryptionController.EncryptAES(new EncryptAESRequest()
            {
                AesType = 256,
                DataToEncrypt = testString,
                NonceKey = "TestingNonce"
            }) as OkObjectResult;
            EncryptAESResponse response = JsonConvert.DeserializeObject<EncryptAESResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.NotEqual(testString, response.Encrypted);
            Assert.NotNull(response.Key);
            Assert.NotNull(response.Nonce);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task DecryptAES()
        {
            string testString = "This is a test string";
            OkObjectResult result = await this._encryptionController.EncryptAES(new EncryptAESRequest() { AesType = 256, DataToEncrypt = testString, NonceKey = "TestingNonce" }) as OkObjectResult;
            EncryptAESResponse response = JsonConvert.DeserializeObject<EncryptAESResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult decryptResult = await this._encryptionController.DecryptAES(new DecryptAESRequest() { AesType = 256, DataToDecrypt = response.Encrypted, Key = response.Key, NonceKey = response.Nonce }) as OkObjectResult;
            DecryptAESResponse decryptResponse = JsonConvert.DeserializeObject<DecryptAESResponse>(JsonConvert.SerializeObject(decryptResult.Value));
            Assert.Equal(testString, decryptResponse.Decrypted);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(200, decryptResult.StatusCode);
        }

        [Fact]
        public async Task HashSHA512()
        {
            string testDataToEncrypt = "HashThisWithSHA512";
            EncryptSHARequest request = new EncryptSHARequest() { DataToEncrypt = testDataToEncrypt };
            OkObjectResult response = await this._encryptionController.EncryptSHA512(request) as OkObjectResult;
            HashSHAResponse responseParsed = JsonConvert.DeserializeObject<HashSHAResponse>(JsonConvert.SerializeObject(response.Value));
            Assert.Equal(200, response.StatusCode);
            Assert.NotEqual(testDataToEncrypt, responseParsed.Hash);
        }

        [Fact]
        public async Task HashSHA256()
        {
            string testDataToEncrypt = "HashThisWithSHA256";
            EncryptSHARequest request = new EncryptSHARequest() { DataToEncrypt = testDataToEncrypt };
            OkObjectResult response = await this._encryptionController.EncryptSHA256(request) as OkObjectResult;
            HashSHAResponse responseParsed = JsonConvert.DeserializeObject<HashSHAResponse>(JsonConvert.SerializeObject(response.Value));
            Assert.Equal(200, response.StatusCode);
            Assert.NotEqual(testDataToEncrypt, responseParsed.Hash);
        }

        [Fact]
        public async Task AESRSAHybridEncryptRequest()
        {
            string dataToEncrypt = "DataToEncrypt";
            AESRSAHybridEncryptRequest request = new AESRSAHybridEncryptRequest()
            {
                AesType = 256,
                DataToEncrypt = dataToEncrypt,
                KeySize = 4096,
                Nonce = "TestingNonce"
            };
            OkObjectResult result = await this._encryptionController.EncryptAESRSAHybrid(request) as OkObjectResult;
            AESRSAHybridEncryptResponse response = JsonConvert.DeserializeObject<AESRSAHybridEncryptResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(200, result.StatusCode);
            Assert.NotEqual(dataToEncrypt, response.EncryptedData);

        }

        [Fact]
        public async Task AESRSAHybridDecryptRequest()
        {
            string dataToEncrypt = "TestDataToDecrypt";
            AESRSAHybridEncryptRequest request = new AESRSAHybridEncryptRequest()
            {
                AesType = 256,
                DataToEncrypt = dataToEncrypt,
                KeySize = 4096,
                Nonce = "TestingNonce"
            };
            OkObjectResult result = await this._encryptionController.EncryptAESRSAHybrid(request) as OkObjectResult;
            AESRSAHybridEncryptResponse response = JsonConvert.DeserializeObject<AESRSAHybridEncryptResponse>(JsonConvert.SerializeObject(result.Value));
            OkObjectResult decryptResult = await this._encryptionController.DecryptAESRSAHybrid(new AESRSAHybridDecryptRequest()
            {
                AesType = 256,
                EncryptedData = response.EncryptedData,
                EncryptedAesKey = response.EncryptedAesKey,
                Nonce = "TestingNonce",
                PrivateRsaKey = response.PrivateKey
            }) as OkObjectResult;
            AESRSAHybridDecryptResponse decryptResponse = JsonConvert.DeserializeObject<AESRSAHybridDecryptResponse>(JsonConvert.SerializeObject(decryptResult.Value));
            Assert.Equal(200, decryptResult.StatusCode);
            Assert.Equal(dataToEncrypt, decryptResponse.DecryptedData);
        }

        [Fact]
        public async Task MD5Hash()
        {
            MD5Request request = new MD5Request()
            {
                DataToHash = "HashThisData"
            };
            OkObjectResult result = await this._encryptionController.HashMD5(request) as OkObjectResult;
            MD5HashResponse hashResponse = JsonConvert.DeserializeObject<MD5HashResponse>(JsonConvert.SerializeObject(result.Value));
            Assert.Equal(200, result.StatusCode);
            Assert.NotEqual(request.DataToHash, hashResponse.Hash);
        }

        [Fact]
        public async Task MD5HashVerify()
        {
            MD5Request request = new MD5Request()
            {
                DataToHash = "HashThisData"
            };
            OkObjectResult hashResult = await this._encryptionController.HashMD5(request) as OkObjectResult;
            MD5HashResponse hashResponse = JsonConvert.DeserializeObject<MD5HashResponse>(JsonConvert.SerializeObject(hashResult.Value));
            MD5VerifyRequest verifyRequest = new MD5VerifyRequest()
            {
                HashToVerify = hashResponse.Hash,
                ToHash = "HashThisData"
            };
            OkObjectResult verifyResult = await this._encryptionController.VerifyMD5(verifyRequest) as OkObjectResult;
            MD5VerifyResponse verifyResponse = JsonConvert.DeserializeObject<MD5VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(200, verifyResult.StatusCode);
            Assert.Equal(true, verifyResponse.IsValid);
        }

        [Fact]
        public async Task Blake2512Hash()
        {
            Blake2Request blake2Request = new Blake2Request()
            {
                HashSize = 512,
                DataToHash = "hello world"
            };
            OkObjectResult hashResult = await this._encryptionController.Blake2Hash(blake2Request) as OkObjectResult;
            Blake2Response hashResponse = JsonConvert.DeserializeObject<Blake2Response>(JsonConvert.SerializeObject(hashResult.Value));
            Assert.Equal(200, hashResult.StatusCode);
            Assert.NotNull(hashResponse.HashedData);
            Assert.NotEqual(hashResponse.HashedData, blake2Request.DataToHash);
            Assert.Equal(hashResponse.HashedData, "Ahzth5kpbOylV4MquUGlC0oR+DR4zxQfUfkz9lOrn7zAWgN83b7QbjCb8zSULE5YzfGkbiN5EczX/Pl4fLx/0A==");
        }

        [Fact]
        public async Task Blake2512Verify()
        {
            Blake2Request blake2Request = new Blake2Request()
            {
                HashSize = 512,
                DataToHash = "hello world"
            };
            OkObjectResult hashResult = await this._encryptionController.Blake2Hash(blake2Request) as OkObjectResult;
            Blake2Response hashResponse = JsonConvert.DeserializeObject<Blake2Response>(JsonConvert.SerializeObject(hashResult.Value));
            Blake2VerifyRequest verifyRequest = new Blake2VerifyRequest()
            {
                HashSize = 512,
                Hash = hashResponse.HashedData,
                DataToVerify = "hello world"
            };
            OkObjectResult verifyResult = await this._encryptionController.Blake2Verify(verifyRequest) as OkObjectResult;
            Blake2VerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(200, verifyResult.StatusCode);
            Assert.Equal(true, verifyResponse.IsValid);
        }

        [Fact]
        public async Task Blake2256Hash()
        {
            Blake2Request blake2Request = new Blake2Request()
            {
                HashSize = 256,
                DataToHash = "hello world"
            };
            OkObjectResult hashResult = await this._encryptionController.Blake2Hash(blake2Request) as OkObjectResult;
            Blake2Response hashResponse = JsonConvert.DeserializeObject<Blake2Response>(JsonConvert.SerializeObject(hashResult.Value));
            Assert.Equal(200, hashResult.StatusCode);
            Assert.NotNull(hashResponse.HashedData);
            Assert.NotEqual(hashResponse.HashedData, blake2Request.DataToHash);
            Assert.Equal(hashResponse.HashedData, "muxoBnlFYRB+WUsfaoprDJKgy6ms9eXpPMoG94GBOws=");
        }

        [Fact]
        public async Task Blake2256Verify()
        {
            Blake2Request blake2Request = new Blake2Request()
            {
                HashSize = 256,
                DataToHash = "hello world"
            };
            OkObjectResult hashResult = await this._encryptionController.Blake2Hash(blake2Request) as OkObjectResult;
            Blake2Response hashResponse = JsonConvert.DeserializeObject<Blake2Response>(JsonConvert.SerializeObject(hashResult.Value));
            Blake2VerifyRequest verifyRequest = new Blake2VerifyRequest()
            {
                HashSize = 256,
                Hash = hashResponse.HashedData,
                DataToVerify = "hello world"
            };
            OkObjectResult verifyResult = await this._encryptionController.Blake2Verify(verifyRequest) as OkObjectResult;
            Blake2VerifyResponse verifyResponse = JsonConvert.DeserializeObject<Blake2VerifyResponse>(JsonConvert.SerializeObject(verifyResult.Value));
            Assert.Equal(200, verifyResult.StatusCode);
            Assert.Equal(true, verifyResponse.IsValid);
        }
    }
}
