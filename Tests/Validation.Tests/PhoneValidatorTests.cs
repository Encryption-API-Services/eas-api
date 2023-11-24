using Validation.Phone;
using Xunit;

namespace Validation.Tests
{
    public class PhoneValidatorTests
    {
        private PhoneValidator _validator;
        public PhoneValidatorTests()
        {
            this._validator = new PhoneValidator();
        }

        [Fact]
        public void IsPhoneValid()
        {
            string phoneNumber = "7086015092";
            bool isValid = this._validator.IsPhoneNumberValid(phoneNumber);
            Assert.True(isValid);
        }
    }
}
