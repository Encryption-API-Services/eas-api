using Encryption.PasswordHash;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class BcryptWrapperTests
    {
        private BcryptWrapper _cryptWrapper { get; set; }
        private string _testPassword { get; set; }

        public BcryptWrapperTests()
        {
            this._cryptWrapper = new BcryptWrapper();
            this._testPassword = "testPassword";
        }

        [Fact]
        public void HashPassword()
        {
            IntPtr hashedPasswordPtr = this._cryptWrapper.HashPassword(this._testPassword);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            BcryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.NotEqual(hashedPassword, this._testPassword);
        }

        [Fact]
        public async Task HashPasswordAsync()
        {
            IntPtr hashedPasswordPtr = await this._cryptWrapper.HashPasswordAsync(this._testPassword);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            BcryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.NotEqual(hashedPassword, this._testPassword);
        }

        [Fact]
        public async Task Verify()
        {
            IntPtr hashedPasswordPtr = this._cryptWrapper.HashPassword(this._testPassword);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            BcryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.True(this._cryptWrapper.Verify(hashedPassword, this._testPassword));
        }

        [Fact]
        public async Task VerifyAsync()
        {
            IntPtr hashedPasswordPtr = await this._cryptWrapper.HashPasswordAsync(this._testPassword);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            BcryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.True(await this._cryptWrapper.VerifyAsync(hashedPassword, this._testPassword));
        }

        [Fact]
        public async Task VerifyFailAsync()
        {
            IntPtr hashedPasswordPtr = await this._cryptWrapper.HashPasswordAsync(this._testPassword);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            BcryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.True(!await this._cryptWrapper.VerifyAsync(hashedPassword, "1234"));
        }
    }
}
