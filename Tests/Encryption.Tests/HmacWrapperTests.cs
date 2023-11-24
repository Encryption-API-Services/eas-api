using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class HmacWrapperTests
    {
        private HmacWrapper _hmacWrapper { get; set; }
        public HmacWrapperTests()
        {
            this._hmacWrapper = new HmacWrapper();
        }

        [Fact]
        public void HmacSign()
        {
            string key = "HmacKey";
            string message = "message to sign";
            IntPtr signaturePtr = this._hmacWrapper.HmacSign(key, message);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            HmacWrapper.free_cstring(signaturePtr);
            Assert.NotNull(signature);
            Assert.NotEqual(message, signature);
        }

        [Fact]
        public async Task HmacSignAsync()
        {
            string key = "HmacKey";
            string message = "message to sign";
            IntPtr signaturePtr = await this._hmacWrapper.HmacSignAsync(key, message);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            HmacWrapper.free_cstring(signaturePtr);
            Assert.NotNull(signature);
            Assert.NotEqual(message, signature);
        }

        [Fact]
        public void HmacVerify()
        {
            string key = "HmacKey";
            string message = "message to sign";
            IntPtr signaturePtr = this._hmacWrapper.HmacSign(key, message);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            bool isValid = this._hmacWrapper.HmacVerify(key, message, signature);
            HmacWrapper.free_cstring(signaturePtr);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async Task HmacVerifyAsync()
        {
            string key = "HmacKey";
            string message = "message to sign";
            IntPtr signaturePtr = this._hmacWrapper.HmacSign(key, message);
            string signature = Marshal.PtrToStringAnsi(signaturePtr);
            bool isValid = await this._hmacWrapper.HmacVerifyAsync(key, message, signature);
            HmacWrapper.free_cstring(signaturePtr);
            Assert.Equal(true, isValid);
        }
    }
}
