using Models.Credit;
using Xunit;

namespace Models.Tests
{
    public class CreditCardValidation
    {
        [Fact]
        public void CreateCreditValidateRequest()
        {
            CreditValidateRequest request = new CreditValidateRequest()
            {
                CCNumber = "4422123443211234"
            };
            Assert.NotNull(request);
            Assert.NotNull(request.CCNumber);
        }
    }
}