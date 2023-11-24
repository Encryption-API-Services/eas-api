using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments
{
    public class StripSubscription
    {
        public async Task<Subscription> CreateSubscription(string stripCustomerId, string stripPriceId)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            SubscriptionCreateOptions options = new SubscriptionCreateOptions()
            {
                Customer = stripCustomerId,
                Items = new List<SubscriptionItemOptions>()
                {
                    new SubscriptionItemOptions
                    {
                        Price = stripPriceId,
                    }
                }
            };
            SubscriptionService service = new SubscriptionService();
            return await service.CreateAsync(options);
        }

        public async Task UpdateSubscription(string subscriptionId, string priceId)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            SubscriptionUpdateOptions options = new SubscriptionUpdateOptions()
            {
                Items = new List<SubscriptionItemOptions>()
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    }
                },
                ProrationDate = DateTime.UtcNow,
            };
            SubscriptionService service = new SubscriptionService();
            await service.UpdateAsync(subscriptionId, options);
        }

        public async Task CancelSubscription(string subscriptionId)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            SubscriptionService service = new SubscriptionService();
            await service.CancelAsync(subscriptionId);
        }
    }
}
