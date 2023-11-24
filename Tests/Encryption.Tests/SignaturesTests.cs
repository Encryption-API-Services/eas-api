using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using static Encryption.ED25519Wrapper;

namespace Encryption.Tests
{
    public class SignaturesTests
    {
        private readonly ED25519Wrapper _ed25519Wraper;
        private readonly RustSHAWrapper _shaWrapper;

        public SignaturesTests()
        {
            this._ed25519Wraper = new ED25519Wrapper();
            this._shaWrapper = new RustSHAWrapper();
        }

        [Fact]
        public async Task SHA512ED25519DalekSignature()
        {
            string dataToHash = "ShaDataToHash";
            IntPtr hashedDataPtr = await this._shaWrapper.SHA512HashStringAsync(dataToHash);
            string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr);
            IntPtr ed25519KeyPair = this._ed25519Wraper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(ed25519KeyPair);
            Ed25519SignatureResult signatureResult = this._ed25519Wraper.Sign(keyPair, hashedData);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key);
            RustSHAWrapper.free_cstring(hashedDataPtr);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            Assert.NotNull(signature);
            Assert.NotNull(publicKey);
            Assert.NotNull(hashedData);
            Assert.NotEqual(signature, hashedData);
        }

        [Fact]
        public async Task SHA512ED25519DalkeSignatureAsync()
        {
            string dataToHash = "ShaDataToHash";
            IntPtr hashedDataPtr = await this._shaWrapper.SHA512HashStringAsync(dataToHash);
            string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr);
            IntPtr ed25519KeyPair = this._ed25519Wraper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(ed25519KeyPair);
            Ed25519SignatureResult signatureResult = await this._ed25519Wraper.SignAsync(keyPair, hashedData);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key);
            RustSHAWrapper.free_cstring(hashedDataPtr);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            Assert.NotNull(signature);
            Assert.NotNull(publicKey);
            Assert.NotNull(hashedData);
            Assert.NotEqual(signature, hashedData);
        }

        [Fact]
        public async Task SHA512ED25519DalekVerify()
        {
            string dataToHash = "ShaDataToHash";
            IntPtr hashedDataPtr = await this._shaWrapper.SHA512HashStringAsync(dataToHash);
            string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr);
            IntPtr ed25519KeyPair = this._ed25519Wraper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(ed25519KeyPair);
            Ed25519SignatureResult signatureResult = this._ed25519Wraper.Sign(keyPair, hashedData);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key);
            bool isValid = this._ed25519Wraper.VerifyWithPublicKey(publicKey, signature, hashedData);
            RustSHAWrapper.free_cstring(hashedDataPtr);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async Task SHA512ED25519DalekVerifyAsync()
        {
            string dataToHash = "ShaDataToHash";
            IntPtr hashedDataPtr = await this._shaWrapper.SHA512HashStringAsync(dataToHash);
            string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr);
            IntPtr ed25519KeyPair = this._ed25519Wraper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(ed25519KeyPair);
            Ed25519SignatureResult signatureResult = await this._ed25519Wraper.SignAsync(keyPair, hashedData);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key);
            bool isValid = await this._ed25519Wraper.VerifyWithPublicAsync(publicKey, signature, hashedData);
            RustSHAWrapper.free_cstring(hashedDataPtr);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            Assert.Equal(true, isValid);
        }
    }
}