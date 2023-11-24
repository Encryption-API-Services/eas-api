using Models.UserAuthentication;
using System;
using Xunit;

namespace Models.Tests
{
    public class UserAuthentication
    {
        private string _userName = "testuseraccount";
        private string _password = "testpassword12345";

        [Fact]
        public void CreateRegisterUserModel()
        {
            RegisterUser user = new RegisterUser()
            {
                username = this._userName,
                password = this._password
            };

            Assert.Equal(this._userName, user.username);
            Assert.Equal(this._password, user.password);
        }

        [Fact]
        public void CreateActivateUser()
        {
            string id = Guid.NewGuid().ToString();
            string token = Guid.NewGuid().ToString();

            ActivateUser user = new ActivateUser()
            {
                Id = id,
                Token = token
            };

            Assert.Equal(user.Id, id);
            Assert.Equal(user.Token, token);
        }

        [Fact]
        public void CreateForgotPasswordRequest()
        {
            ForgotPasswordRequest request = new ForgotPasswordRequest()
            {
                Email = "testing@outlook.com"
            };

            Assert.NotNull(request);
            Assert.NotNull(request.Email);
        }

        [Fact]
        public void CreateLoginUser()
        {
            LoginUser user = new LoginUser()
            {
                Email = "testEmailW@gmail.com",
                Password = "testPassword123@#"
            };
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Password);
        }

        [Fact]
        public void CreateResetPassword()
        {
            ResetPasswordRequest request = new ResetPasswordRequest()
            {
                Id = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Password = "testingPassword",
                ConfirmPassword = "testingPassword"
            };

            Assert.Equal(request.Password, request.ConfirmPassword);
            Assert.NotNull(request.Password);
            Assert.NotNull(request.Token);
            Assert.NotNull(request.Id);
        }

        [Fact]
        public void CreateUnlockUser()
        {
            UnlockUser user = new UnlockUser()
            {
                Id = Guid.NewGuid().ToString()
            };
            Assert.NotNull(user.Id);
        }
    }
}