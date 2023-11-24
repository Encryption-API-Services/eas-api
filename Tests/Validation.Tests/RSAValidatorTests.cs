using Encryption;
using Encryption.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Validation.Keys;
using Xunit;
using static Encryption.RustRSAWrapper;

namespace Validation.Tests
{
    public class RSAValidatorTests
    {
        private readonly RSAValidator _rsaValidator;
        private readonly RustRSAWrapper _rsaWrapper;
        public RSAValidatorTests()
        {
            this._rsaValidator = new RSAValidator();
            this._rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
        }

        [Fact]
        public async Task RSA1024KeyValidator()
        {
            RustRsaKeyPair keyPair = await this._rsaWrapper.GetKeyPairAsync(1024);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(privateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(publicKey));
        }

        [Fact]
        public async Task RSA2048KeyValidator()
        {
            RustRsaKeyPair keyPair = await this._rsaWrapper.GetKeyPairAsync(2048);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(privateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(publicKey));
        }

        [Fact]
        public async Task RSA4096KeyValidator()
        {
            RustRsaKeyPair keyPair = await this._rsaWrapper.GetKeyPairAsync(4096);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            Assert.True(this._rsaValidator.IsPrivateKeyPEMValid(privateKey));
            Assert.True(this._rsaValidator.IsPublicKeyPEMValid(publicKey));
        }
    }
}
