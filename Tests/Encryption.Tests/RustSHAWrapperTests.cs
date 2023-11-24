using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class RustSHAWrapperTests
    {
        private RustSHAWrapper _wrapper;
        private string _testString;
        public RustSHAWrapperTests()
        {
            this._wrapper = new RustSHAWrapper();
            this._testString = "Test hash to hash";
        }

        [Fact]
        public void SHA512Hash()
        {
            IntPtr hashedPtr = this._wrapper.SHA512HashString(this._testString);
            string hashed = Marshal.PtrToStringAnsi(hashedPtr);
            RustSHAWrapper.free_cstring(hashedPtr);
            Assert.NotNull(hashed);
            Assert.NotEmpty(hashed);
            Assert.NotEqual(hashed, this._testString);
        }

        [Fact]
        public async Task SHA512HashAsync()
        {
            IntPtr hashedPtr = await this._wrapper.SHA512HashStringAsync(this._testString);
            string hashed = Marshal.PtrToStringAnsi(hashedPtr);
            RustSHAWrapper.free_cstring(hashedPtr);
            Assert.NotNull(hashed);
            Assert.NotEmpty(hashed);
            Assert.NotEqual(hashed, this._testString);
        }

        [Fact]
        public async Task SHA256Hash()
        {
            IntPtr hashedPtr = this._wrapper.SHA256HashString(this._testString);
            string hashed = Marshal.PtrToStringAnsi(hashedPtr);
            RustSHAWrapper.free_cstring(hashedPtr);
            Assert.NotNull(hashed);
            Assert.NotEmpty(hashed);
            Assert.NotEqual(hashed, this._testString);
        }

        [Fact]
        public async Task SHA256HashAsync()
        {
            IntPtr hashedPtr = await this._wrapper.SHA256HashStringAsync(this._testString);
            string hashed = Marshal.PtrToStringAnsi(hashedPtr);
            RustSHAWrapper.free_cstring(hashedPtr);
            Assert.NotNull(hashed);
            Assert.NotEmpty(hashed);
            Assert.NotEqual(hashed, this._testString);
        }
    }
}
