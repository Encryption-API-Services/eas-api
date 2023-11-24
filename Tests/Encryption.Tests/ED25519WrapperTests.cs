using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using static Encryption.ED25519Wrapper;

namespace Encryption.Tests
{
    public class ED25519WrapperTests
    {
        private readonly ED25519Wrapper _wrapper;
        public ED25519WrapperTests()
        {
            this._wrapper = new ED25519Wrapper();
        }

        [Fact]
        public void GetKeyPair()
        {
            IntPtr keyPairPtr = this._wrapper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            ED25519Wrapper.free_cstring(keyPairPtr);
            Assert.NotNull(keyPair);
        }

        [Fact]
        public async Task GetKeyPairAsync()
        {
            IntPtr keyPairPtr = await this._wrapper.GetKeyPairAsync();
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            ED25519Wrapper.free_cstring(keyPairPtr);
            Assert.NotNull(keyPair);
        }

        [Fact]
        public void SignData()
        {
            IntPtr keyPairPtr = this._wrapper.GetKeyPair();
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult signedData = this._wrapper.Sign(keyPair, "SignThisData");
            string signature = Marshal.PtrToStringAnsi(signedData.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signedData.Public_Key);
            ED25519Wrapper.free_cstring(keyPairPtr);
            ED25519Wrapper.free_cstring(signedData.Public_Key);
            ED25519Wrapper.free_cstring(signedData.Signature);
            Assert.NotNull(signature);
            Assert.NotNull(publicKey);
        }

        [Fact]
        public async Task SignDataAsync()
        {
            IntPtr keyPairPtr = await this._wrapper.GetKeyPairAsync();
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult signedData = this._wrapper.Sign(keyPair, "SignThisData");
            string signature = Marshal.PtrToStringAnsi(signedData.Signature);
            string publicKey = Marshal.PtrToStringAnsi(signedData.Public_Key);
            ED25519Wrapper.free_cstring(keyPairPtr);
            ED25519Wrapper.free_cstring(signedData.Signature);
            ED25519Wrapper.free_cstring(signedData.Public_Key);
            Assert.NotNull(signature);
            Assert.NotNull(publicKey);
        }

        [Fact]
        public void Verify()
        {
            IntPtr keyPairPtr = this._wrapper.GetKeyPair();
            string dataToSign = "TestData12345";
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult signatureResult = this._wrapper.Sign(keyPair, dataToSign);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            bool isValid = this._wrapper.Verify(keyPair, signature, dataToSign);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            ED25519Wrapper.free_cstring(keyPairPtr);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async Task VerifyAsync()
        {
            IntPtr keyPairPtr = await this._wrapper.GetKeyPairAsync();
            string dataToSign = "TestData12345";
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult signatureResult = await this._wrapper.SignAsync(keyPair, dataToSign);
            string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
            bool isValid = await this._wrapper.VerifyAsync(keyPair, signature, dataToSign);
            ED25519Wrapper.free_cstring(keyPairPtr);
            ED25519Wrapper.free_cstring(signatureResult.Signature);
            ED25519Wrapper.free_cstring(signatureResult.Public_Key);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async void VerifyWithPublicKey()
        {
            IntPtr keyPairPtr = this._wrapper.GetKeyPair();
            string dataToSign = "welcomeHome";
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult result = this._wrapper.Sign(keyPair, dataToSign);
            string publicKey = Marshal.PtrToStringAnsi(result.Public_Key);
            string siganture = Marshal.PtrToStringAnsi(result.Signature);
            bool isValid = this._wrapper.VerifyWithPublicKey(publicKey, siganture, dataToSign);
            ED25519Wrapper.free_cstring(keyPairPtr);
            ED25519Wrapper.free_cstring(result.Public_Key);
            ED25519Wrapper.free_cstring(result.Signature);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async Task VerifyWithPublicAsync()
        {
            IntPtr keyPairPtr = await this._wrapper.GetKeyPairAsync();
            string dataToSign = "welcomeHome";
            string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
            Ed25519SignatureResult result = await this._wrapper.SignAsync(keyPair, dataToSign);
            string publicKey = Marshal.PtrToStringAnsi(result.Public_Key);
            string signature = Marshal.PtrToStringAnsi(result.Signature);
            bool isValid = await this._wrapper.VerifyWithPublicAsync(publicKey, signature, dataToSign);
            Assert.Equal(true, isValid);
        }
    }
}