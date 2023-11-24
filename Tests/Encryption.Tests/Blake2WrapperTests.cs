using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class Blake2WrapperTests
    {
        private readonly Blake2Wrapper _wrapper;

        public Blake2WrapperTests()
        {
            this._wrapper = new Blake2Wrapper();
        }

        [Fact]
        public void Blake2512Hash()
        {
            string message = "hello world";
            IntPtr hashPtr = this._wrapper.Blake2512(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            Assert.NotNull(hash);
            Assert.NotEqual(message, hash);
            Assert.Equal(hash, "Ahzth5kpbOylV4MquUGlC0oR+DR4zxQfUfkz9lOrn7zAWgN83b7QbjCb8zSULE5YzfGkbiN5EczX/Pl4fLx/0A==");
        }

        [Fact]
        public void Blake2256Hash()
        {
            string message = "hello world";
            IntPtr hashPtr = this._wrapper.Blake2256(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            Assert.NotNull(hash);
            Assert.NotEqual(message, hash);
            Assert.Equal(hash, "muxoBnlFYRB+WUsfaoprDJKgy6ms9eXpPMoG94GBOws=");
        }

        [Fact]
        public async Task Blake2512HashAsync()
        {
            string message = "hello world";
            IntPtr hashPtr = await this._wrapper.Blake2512Async(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            Assert.NotNull(hash);
            Assert.NotEqual(message, hash);
            Assert.Equal(hash, "Ahzth5kpbOylV4MquUGlC0oR+DR4zxQfUfkz9lOrn7zAWgN83b7QbjCb8zSULE5YzfGkbiN5EczX/Pl4fLx/0A==");
        }

        [Fact]
        public async Task Blake2256HashAsync()
        {
            string message = "hello world";
            IntPtr hashPtr = await this._wrapper.Blake2256Async(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            Assert.NotNull(hash);
            Assert.NotEqual(message, hash);
            Assert.Equal(hash, "muxoBnlFYRB+WUsfaoprDJKgy6ms9eXpPMoG94GBOws=");
        }

        [Fact]
        public void Blake2512VerifyPass()
        {
            string message = "hello world";
            string messageToVerify = "hello world";
            IntPtr hashPtr = this._wrapper.Blake2512(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            bool result = this._wrapper.Blake2512Verify(messageToVerify, hash);
            Assert.Equal(result, true);
        }

        [Fact]
        public void Blake2512VerifyAsyncFail()
        {
            string message = "hello world";
            string messageToVerify = "hello worl";
            IntPtr hashPtr = this._wrapper.Blake2512(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            bool result = this._wrapper.Blake2512Verify(messageToVerify, hash);
            Assert.Equal(result, false);
        }

        [Fact]
        public void Blake2256VerifyPass()
        {
            string message = "hello world";
            string messageToVerify = "hello world";
            IntPtr hashPtr = this._wrapper.Blake2256(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            bool result = this._wrapper.Blake2256Verify(messageToVerify, hash);
            Assert.Equal(result, true);
        }

        [Fact]
        public void Blake2256VerifyFailAsync()
        {
            string message = "hello world";
            string messageToVerify = "hello worl";
            IntPtr hashPtr = this._wrapper.Blake2256(message);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            Blake2Wrapper.free_cstring(hashPtr);
            bool result = this._wrapper.Blake2256Verify(messageToVerify, hash);
            Assert.Equal(result, false);
        }
    }
}