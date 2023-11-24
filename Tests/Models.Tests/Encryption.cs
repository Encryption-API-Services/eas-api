using Models.Encryption;
using Xunit;

namespace Models.Tests
{
    public class Encryption
    {
        public Encryption()
        {

        }

        [Fact]
        public void CreateBCryptModel()
        {
            BCryptEncryptModel model = new BCryptEncryptModel()
            {
                Password = "testpassword"
            };
            Assert.NotNull(model);
            Assert.NotNull(model.Password);
        }

        [Fact]
        public void CreateBcryptVerifyModel()
        {
            BcryptVerifyModel model = new BcryptVerifyModel()
            {
                Password = "testpassword",
            };
            Assert.NotNull(model);
            Assert.NotNull(model.Password);
        }

        [Fact]
        public void CreateDecryptAESRequest()
        {
            DecryptAESRequest model = new DecryptAESRequest()
            {
                DataToDecrypt = "Data to encryption data",
                Key = "testpassword",
                NonceKey = "TestNonce",
            };
            Assert.NotNull(model);
            Assert.NotNull(model.Key);
            Assert.NotNull(model.DataToDecrypt);
            Assert.NotNull(model.NonceKey);
        }

        [Fact]
        public void CreateEncryptSHARequest()
        {
            EncryptSHARequest model = new EncryptSHARequest()
            {
                DataToEncrypt = "testpassword",

            };
            Assert.NotNull(model.DataToEncrypt);
        }

        [Fact]
        public void CreateEncryptAESRequest()
        {
            EncryptAESRequest model = new EncryptAESRequest()
            {
                DataToEncrypt = "sha that data bad boy"
            };
            Assert.NotNull(model);
            Assert.NotNull(model.DataToEncrypt);
        }
    }
}
