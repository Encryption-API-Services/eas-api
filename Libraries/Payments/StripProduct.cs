using Stripe;
using System;
using System.Threading.Tasks;

namespace Payments
{
    public class StripProduct
    {


        public async Task<StripeList<Product>> GetStripProductList()
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            ProductService service = new ProductService();
            return await service.ListAsync();
        }

        public async Task<Product> CreateProduct(string productName)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripApiKey");
            ProductCreateOptions options = new ProductCreateOptions()
            {
                Name = productName,
            };
            ProductService service = new ProductService();
            return await service.CreateAsync(options);
        }
    }
}
