using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Asymmetric.Types;
using System.Threading.Tasks;
using Validation.Keys;
using Xunit;

namespace Validation.Tests
{
    public class RSAValidatorTests
    {
        private readonly RSAValidator _rsaValidator;
        private readonly RSAWrapper _rsaWrapper;
        public RSAValidatorTests()
        {
            this._rsaValidator = new RSAValidator();
            this._rsaWrapper = new RSAWrapper();
        }

        [Fact]
        public async Task RSA1024KeyValidator()
        {
            RsaKeyPairResult keyPair = this._rsaWrapper.GetKeyPair(1024);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(keyPair.PrivateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(keyPair.PublicKey));
        }

        [Fact]
        public async Task RSA2048KeyValidator()
        {
            RsaKeyPairResult keyPair = this._rsaWrapper.GetKeyPair(2048);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(keyPair.PrivateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(keyPair.PublicKey));
        }

        [Fact]
        public async Task RSA4096KeyValidator()
        {
            RsaKeyPairResult keyPair = this._rsaWrapper.GetKeyPair(4096);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(keyPair.PrivateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(keyPair.PublicKey));
        }
    }
}
