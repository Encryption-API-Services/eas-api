using DataLayer.Mongo.Entities;
using Models.Payments;
using Stripe;
using System;
using System.Threading.Tasks;

namespace Payments
{
    public class StripCustomer
    {
        public StripCustomer()
        {

        }
        public async Task<string> CreateStripCustomer(User user)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            CustomerCreateOptions options = new CustomerCreateOptions
            {
                Description = String.Format("The user has the username {0} in the Encryption API Services data", user.Username),
                Email = user.Email,
                Address = new AddressOptions()
                {
                    Line1 = user.BillingInformation.AddressOne,
                    Line2 = user.BillingInformation.AddressTwo,
                    City = user.BillingInformation.City,
                    State = user.BillingInformation.State,
                    PostalCode = user.BillingInformation.Zip,
                    Country = user.BillingInformation.Country
                }
            };
            CustomerService customerService = new CustomerService();
            Customer customer = await customerService.CreateAsync(options);
            return customer.Id;
        }

        public async Task DeleteStripCustomer(string stripUserId)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            CustomerService customerService = new CustomerService();
            Customer customer = await customerService.DeleteAsync(stripUserId);
        }

        public async Task UpdateStripeCustomerBillingInformation(string stripeCustomerId, UpdateBillingInformationRequestBody billingInfo)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            CustomerUpdateOptions options = new CustomerUpdateOptions()
            {
                Address = new AddressOptions()
                {
                    Line1 = billingInfo.AddressOne,
                    Line2 = billingInfo.AddressTwo,
                    City = billingInfo.City,
                    State = billingInfo.State,
                    PostalCode = billingInfo.Zip,
                    Country = billingInfo.Country
                }
            };
            CustomerService customerService = new CustomerService();
            await customerService.UpdateAsync(stripeCustomerId, options);
        }
    }
}