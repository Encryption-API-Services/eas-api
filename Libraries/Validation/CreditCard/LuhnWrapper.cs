using LuhnNet;
using System.Threading.Tasks;

namespace Validation.CreditCard
{
    public class LuhnWrapper
    {
        public bool IsCCValid(string ccNumber)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(ccNumber))
            {
                result = Luhn.IsValid(ccNumber);
            }
            return result;
        }
        public async Task<bool> IsCCValidAsync(string ccNumber)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(ccNumber))
            {
                result = await Task.Run(() =>
                {
                    return Luhn.IsValid(ccNumber);
                });
            }
            return result;
        }
    }
}
