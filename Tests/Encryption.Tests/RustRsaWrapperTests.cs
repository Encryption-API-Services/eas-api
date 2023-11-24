using Encryption.Compression;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using static Encryption.RustRSAWrapper;

namespace Encryption.Tests
{
    public class RustRsaWrapperTests
    {
        private readonly RustRSAWrapper _rustRsaWrapper;
        private readonly RustRsaKeyPair _encryptDecryptKeyPair;

        public RustRsaWrapperTests()
        {
            this._rustRsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
            this._encryptDecryptKeyPair = this._rustRsaWrapper.GetKeyPairAsync(4096).GetAwaiter().GetResult();
        }

        [Fact]
        public void CreateKeyPair()
        {
            RustRsaKeyPair keyPair = this._rustRsaWrapper.GetKeyPair(4096);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            Assert.NotNull(privateKey);
            Assert.NotNull(publicKey);
        }

        [Fact]
        public async Task CreateKeyPairAsync()
        {
            RustRsaKeyPair keyPair = await this._rustRsaWrapper.GetKeyPairAsync(4096);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            Assert.NotNull(privateKey);
            Assert.NotNull(publicKey);
        }

        [Fact]
        public void RsaEncrypt()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = this._rustRsaWrapper.RsaEncrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public async Task RsaEncryptAsync()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = await this._rustRsaWrapper.RsaEncryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public void RsaZSTDEncrypt()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = this._rustRsaWrapper.RsaZSTDEncrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public async Task RsaZSTDEncryptAsync()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = await this._rustRsaWrapper.RsaZSTDEncryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public void RsaDecrypt()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = this._rustRsaWrapper.RsaEncrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            IntPtr decryptedPtr = this._rustRsaWrapper.RsaDecrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.priv_key), encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            RustRSAWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public async Task RsaDecryptAsync()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = await this._rustRsaWrapper.RsaEncryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            IntPtr decryptedPtr = await this._rustRsaWrapper.RsaDecryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.priv_key), encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            RustRSAWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public void RsaDecryptZSTD()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = this._rustRsaWrapper.RsaZSTDEncrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            IntPtr decryptedPtr = this._rustRsaWrapper.RsaZSTDDecrypt(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.priv_key), encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            RustRSAWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public async Task RsaDecryptZSTDAsync()
        {
            string dataToEncrypt = "EncryptingStuffIsFun";
            IntPtr encryptedPtr = await this._rustRsaWrapper.RsaZSTDEncryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.pub_key), dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            IntPtr decryptedPtr = await this._rustRsaWrapper.RsaZSTDDecryptAsync(Marshal.PtrToStringAnsi(this._encryptDecryptKeyPair.priv_key), encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            RustRSAWrapper.free_cstring(encryptedPtr);
            RustRSAWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public async void RsaSign()
        {
            string dataToSign = "Sign This Data For Me";
            RsaSignResult result = this._rustRsaWrapper.RsaSign(dataToSign, 4096);
            string publicKey = Marshal.PtrToStringAnsi(result.public_key);
            string signature = Marshal.PtrToStringAnsi(result.signature);
            RustRSAWrapper.free_cstring(result.public_key);
            RustRSAWrapper.free_cstring(result.signature);
            Assert.NotNull(publicKey);
            Assert.NotNull(signature);
        }

        [Fact]
        public async Task RsaSignAsync()
        {
            string dataToSign = "Data That Needs To Be Signed Via RSA";
            RsaSignResult result = await this._rustRsaWrapper.RsaSignAsync(dataToSign, 4096);
            string publicKey = Marshal.PtrToStringAnsi(result.public_key);
            string signature = Marshal.PtrToStringAnsi(result.signature);
            RustRSAWrapper.free_cstring(result.public_key);
            RustRSAWrapper.free_cstring(result.signature);
            Assert.NotNull(publicKey);
            Assert.NotNull(signature);
        }

        [Fact]
        public async void RsaVerify()
        {
            string dataToSign = "Data That Needs To Be Verified";
            RsaSignResult result = this._rustRsaWrapper.RsaSign(dataToSign, 4096);
            string publicKey = Marshal.PtrToStringAnsi(result.public_key);
            string signature = Marshal.PtrToStringAnsi(result.signature);
            bool isValid = this._rustRsaWrapper.RsaVerify(publicKey, dataToSign, signature);
            RustRSAWrapper.free_cstring(result.public_key);
            RustRSAWrapper.free_cstring(result.signature);
            Assert.Equal(true, isValid);
        }


        [Fact]
        public async Task RsaVerifyAsync()
        {
            string dataToSign = "Data That Needs To Be Verified";
            RsaSignResult result = await this._rustRsaWrapper.RsaSignAsync(dataToSign, 4096);
            string publicKey = Marshal.PtrToStringAnsi(result.public_key);
            string signature = Marshal.PtrToStringAnsi(result.signature);
            RustRSAWrapper.free_cstring(result.public_key);
            RustRSAWrapper.free_cstring(result.signature);
            bool isValid = await this._rustRsaWrapper.RsaVerifyAsync(publicKey, dataToSign, signature);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async void RsaSignWithKey()
        {
            string dataToSign = "This data needs to be signed now";
            RustRsaKeyPair keyPair = this._rustRsaWrapper.GetKeyPair(2048);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            IntPtr signaturePtr = this._rustRsaWrapper.RsaSignWithKey(privateKey, dataToSign);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(signaturePtr);
            Assert.NotNull(signature);
            Assert.NotEqual(dataToSign, signature);
        }

        [Fact]
        public async Task RsaSignWithKeyAsync()
        {
            string dataToSign = "This data needs to be signed now";
            RustRsaKeyPair keyPair = await this._rustRsaWrapper.GetKeyPairAsync(2048);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            IntPtr signaturePtr = await this._rustRsaWrapper.RsaSignWithKeyAsync(privateKey, dataToSign);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(signaturePtr);
            Assert.NotNull(signature);
            Assert.NotEqual(dataToSign, signature);
        }
    }
}