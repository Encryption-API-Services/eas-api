using System.Threading.Tasks;
using Validation.CreditCard;
using Xunit;

namespace Validation.Tests
{
    public class LuhnWrapperTests
    {
        private LuhnWrapper _luhnWrapper { get; set; }
        public LuhnWrapperTests()
        {
            this._luhnWrapper = new LuhnWrapper();
        }

        [Fact]
        public void IsCCValid()
        {
            string fakeCC = "347043423418930";
            bool isValid = this._luhnWrapper.IsCCValid(fakeCC);
            Assert.True(isValid);
        }

        [Fact]
        public async Task IsCCValidAsync()
        {
            string fakeCC = "347043423418930";
            bool isValid = await this._luhnWrapper.IsCCValidAsync(fakeCC);
            Assert.True(isValid);
        }
    }
}
