using Validation.UserRegistration;
using Xunit;

namespace Validation.Tests
{
    public class RegisterUserValidationTests
    {
        private RegisterUserValidation _validation { get; set; }

        public RegisterUserValidationTests()
        {
            _validation = new RegisterUserValidation();
        }

        [Fact]
        public void EmailIsValid()
        {
            Assert.Equal(true, this._validation.IsEmailValid("mtmulch@gmail.com"));
        }

        [Fact]
        public void EmailIsNotValid()
        {
            Assert.Equal(false, this._validation.IsEmailValid("mtmulch"));
        }

        [Fact]
        public void UserIsValid()
        {
            Assert.Equal(true, this._validation.IsUserNameValid("Testusername23s"));
        }

        [Fact]
        public void UserIsNotValid()
        {
            Assert.Equal(false, this._validation.IsUserNameValid("12"));
        }

        [Fact]
        public void PasswordIsValid()
        {
            Assert.Equal(true, this._validation.IsPasswordValid("Testing12345!@"));
        }

        [Fact]
        public void PasswordIsNotValid()
        {
            Assert.Equal(false, this._validation.IsPasswordValid("NotaValidPassword"));
        }
    }
}
