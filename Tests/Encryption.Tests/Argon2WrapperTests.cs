using Encryption.PasswordHash;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class Argon2WrapperTests
    {
        private Argon2Wrappper _argon2Wrapper;

        public Argon2WrapperTests()
        {
            this._argon2Wrapper = new Argon2Wrappper();
        }

        [Fact]
        public void HashPassword()
        {
            string password = "DoNotUSETHISPASS@!";
            IntPtr hashPtr = this._argon2Wrapper.HashPassword(password);
            string hash = Marshal.PtrToStringUTF8(hashPtr);
            Argon2Wrappper.free_cstring(hashPtr);
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public async Task HashPasswordAsync()
        {
            string password = "DoNotUSETHISPASS@!";
            IntPtr hashPtr = await this._argon2Wrapper.HashPasswordAsync(password);
            string hash = Marshal.PtrToStringUTF8(hashPtr);
            Argon2Wrappper.free_cstring(hashPtr);
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void Verify()
        {
            string password = "TestPasswordToVerify";
            IntPtr hashPtr = this._argon2Wrapper.HashPassword(password);
            string hash = Marshal.PtrToStringUTF8(hashPtr);
            Argon2Wrappper.free_cstring(hashPtr);
            bool isValid = this._argon2Wrapper.VerifyPassword(hash, password);
            Assert.True(isValid);
        }

        [Fact]
        public async Task VerifyAsync()
        {
            string password = "AsyncTestingToTheMoon!";
            IntPtr hashPtr = await this._argon2Wrapper.HashPasswordAsync(password);
            string hash = Marshal.PtrToStringUTF8(hashPtr);
            Argon2Wrappper.free_cstring(hashPtr);
            bool isValid = await this._argon2Wrapper.VerifyPasswordAsync(hash, password);
            Assert.True(isValid);
        }

        [Fact]
        public async Task VerifyFailAsync()
        {
            string password = "AsyncTestingToTheMoon!";
            string newPassword = "testing!@#$%";
            IntPtr hashPtr = await this._argon2Wrapper.HashPasswordAsync(password);
            string hash = Marshal.PtrToStringUTF8(hashPtr);
            Argon2Wrappper.free_cstring(hashPtr);
            bool isValid = await this._argon2Wrapper.VerifyPasswordAsync(hash, newPassword);
            Assert.True(!isValid);
        }
    }
}
