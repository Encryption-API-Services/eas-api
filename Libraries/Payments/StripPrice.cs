using Stripe;
using System;
using System.Threading.Tasks;

namespace Payments
{
    public class StripPrice
    {
        public async Task<Price> CreateMonthlyUSDPrice(long price, string stripProductId)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            PriceCreateOptions options = new PriceCreateOptions()
            {
                UnitAmount = price * 100,
                Currency = "usd",
                Recurring = new PriceRecurringOptions()
                {
                    Interval = "month",
                },
                Product = stripProductId
            };
            PriceService service = new PriceService();
            return await service.CreateAsync(options);
        }
    }
}
