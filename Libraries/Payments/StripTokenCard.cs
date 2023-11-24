using Stripe;
using System;
using System.Threading.Tasks;

namespace Payments
{
    public class StripTokenCard
    {
        public StripTokenCard()
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
        }
        public async Task<string> CreateTokenCard(string ccNumber, string expMonth, string expYear, string securityCode)
        {
            var options = new TokenCreateOptions()
            {
                Card = new TokenCardOptions()
                {
                    Number = ccNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = securityCode
                }
            };
            var service = new TokenService();
            Token testing = await service.CreateAsync(options);
            return testing.Id;
        }

        public async Task<Card> AddTokenCardToCustomer(string customerId, string tokenId)
        {
            var options = new CardCreateOptions
            {
                Source = tokenId
            };
            var service = new CardService();
            return await service.CreateAsync(customerId, options);
        }

        public async Task DeleteCustomerCard(string customerId, string tokenId)
        {
            var cardService = new CardService();
            Card stripCode = await cardService.GetAsync(customerId, tokenId);
            if (stripCode != null)
            {
                var response = await cardService.DeleteAsync(customerId, tokenId);
            }
        }
    }
}
