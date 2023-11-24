using Encryption.PasswordHash;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Encryption.Tests
{
    public class SCryptWrapperTests
    {
        private readonly SCryptWrapper _scrypt;
        private readonly string _password;
        public SCryptWrapperTests()
        {
            this._scrypt = new SCryptWrapper();
            this._password = "TestPasswordToHash";
        }

        [Fact]
        public void HashPassword()
        {
            IntPtr hashedPasswordPtr = this._scrypt.HashPassword(this._password);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            SCryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(hashedPassword, this._password);
        }

        [Fact]
        public async Task HashPasswordAsync()
        {
            IntPtr hashedPasswordPtr = await this._scrypt.HashPasswordAsync(this._password);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            SCryptWrapper.free_cstring(hashedPasswordPtr);
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(hashedPassword, this._password);
        }

        [Fact]
        public void VerifyPassword()
        {
            IntPtr hashedPasswordPtr = this._scrypt.HashPassword(this._password);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            SCryptWrapper.free_cstring(hashedPasswordPtr);
            bool isValid = this._scrypt.VerifyPassword(this._password, hashedPassword);
            Assert.True(isValid);
        }

        [Fact]
        public async Task VerifyPasswordAsync()
        {
            IntPtr hashedPasswordPtr = await this._scrypt.HashPasswordAsync(this._password);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            SCryptWrapper.free_cstring(hashedPasswordPtr);
            bool isValid = await this._scrypt.VerifyPasswordAsync(this._password, hashedPassword);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public async Task VerifyPasswordFailAsync()
        {
            IntPtr hashedPasswordPtr = await this._scrypt.HashPasswordAsync(this._password);
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            SCryptWrapper.free_cstring(hashedPasswordPtr);
            bool isValid = await this._scrypt.VerifyPasswordAsync("12345", hashedPassword);
            Assert.Equal(false, isValid);
        }
    }
}