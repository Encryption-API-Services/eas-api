using CasDotnetSdk.DigitalSignature;
using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.CustomEntities;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Payments;
using Payments;
using System.Reflection;
using System.Text;
using Twilio.TwiML.Messaging;

namespace API.ControllerLogic
{
    public class PaymentsControllerLogic : IPaymentsControllerLogic
    {
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly IProductRepository _productRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IUserRepository _userRepository;
        public PaymentsControllerLogic(
            ICASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache,
            IProductRepository productRepository,
            IPriceRepository priceRepository,
            IUserRepository userRepository
            )
        {
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchmarkMethodCache;
            this._productRepository = productRepository;
            this._priceRepository = priceRepository;
            this._userRepository = userRepository;
        }

        public async Task<IActionResult> AssignProductToUser(HttpContext context, AssignProductToUserRequestBody body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                User user = await this._userRepository.GetUserById(userId);
                StripSubscription stripSubscription = new StripSubscription();
                if (user.UserSubscriptionSettings.StripSubscriptionId != null)
                {
                    await stripSubscription.CancelSubscription(user.UserSubscriptionSettings.StripSubscriptionId);
                    Stripe.Subscription newSubscription = await stripSubscription.CreateSubscription(user.StripCustomerId, body.StripePriceId);
                    await this._userRepository.UpdateStripeSubscriptionAndProductId(userId, newSubscription.Id, body.StripeProductId);
                    result = new OkObjectResult(new { message = "Subscription updated successfully." });
                }
                else
                {
                    Stripe.Subscription newSubscription = await stripSubscription.CreateSubscription(user.StripCustomerId, body.StripePriceId);
                    await this._userRepository.UpdateStripeSubscriptionAndProductId(userId, newSubscription.Id, body.StripeProductId);
                    result = new OkObjectResult(new { message = "Subscription created successfully." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> CreatePrice(HttpContext context, CreatePriceRequestBody body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (body.Price > 0 && !string.IsNullOrEmpty(body.ProductId))
                {
                    Price databasePrice = await this._priceRepository.GetPriceByAmount(body.Price);
                    if (databasePrice == null)
                    {
                        StripPrice stripPriceService = new StripPrice();
                        Stripe.Price newStripPrice = await stripPriceService.CreateMonthlyUSDPrice(body.Price, body.ProductId);
                        Price newPrice = new Price()
                        {
                            Amount = body.Price,
                            StripeId = newStripPrice.Id,
                            StripeProductId = body.ProductId,
                            CreateDate = DateTime.UtcNow
                        };
                        await this._priceRepository.InsertPrice(newPrice);
                        result = new OkObjectResult(new { message = "Price created successfully." });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = "Price already exists." });
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "Price and product cannot be empty." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> CreateProduct(HttpContext context, CreateProductRequestBody body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.ProductName))
                {
                    Product existingProduct = await this._productRepository.GetProductByName(body.ProductName);
                    if (existingProduct == null)
                    {
                        StripProduct stripProduct = new StripProduct();
                        Stripe.Product newStripeProduct = await stripProduct.CreateProduct(body.ProductName);
                        Product newProduct = new Product()
                        {
                            ProductName = newStripeProduct.Name,
                            StripeId = newStripeProduct.Id,
                            CreateDate = DateTime.UtcNow
                        };
                        await this._productRepository.InsertProduct(newProduct);
                        result = new OkObjectResult(new { message = "Product created successfully." });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = "Product already exists." });
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "Product name cannot be empty." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> DisableSubscription(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                User user = await this._userRepository.GetUserById(userId);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.UserSubscriptionSettings.StripSubscriptionId))
                    {
                        result = new BadRequestObjectResult(new { error = "Cannot disable no subscription" });
                    }
                    else
                    {
                        StripSubscription stripSubscription = new StripSubscription();
                        await stripSubscription.CancelSubscription(user.UserSubscriptionSettings.StripSubscriptionId);
                        await this._userRepository.UpdateStripeSubscriptionToNull(userId);
                        result = new OkObjectResult(new { message = "Subscription disabled successfully." });
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "User not found." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> GetBillingInformation(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                User user = await this._userRepository.GetUserById(userId);
                result = new OkObjectResult(new { BillingInformation = user.BillingInformation });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> GetProducts(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                List<Product> products = await this._productRepository.GetAllProducts();
                result = new OkObjectResult(new { Products = products });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> GetProductsWithPrice(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                Task<User> user = this._userRepository.GetUserById(userId);
                Task<List<Product>> products = this._productRepository.GetAllProducts();
                await Task.WhenAll(user, products);
                List<ProductWithPrices> productWithPrices = new List<ProductWithPrices>();
                foreach (Product product in products.Result)
                {
                    bool isAssignedToMe = (product.StripeId == user.Result.StripProductId) ? true : false;
                    List<Price> prices = await this._priceRepository.GetPricesByProductId(product.StripeId);
                    productWithPrices.Add(new ProductWithPrices() { Product = product, Prices = prices, IsAssignedToMe = isAssignedToMe });
                }
                result = new OkObjectResult(new { Products = productWithPrices });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> UpdateBillingInformation(HttpContext context, UpdateBillingInformationRequestBody body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                Task update = this._userRepository.UpdateBillingInformation(userId, body);
                Task<User> user = this._userRepository.GetUserById(userId);
                await Task.WhenAll(update, user);
                StripCustomer stripCustomer = new StripCustomer();
                await stripCustomer.UpdateStripeCustomerBillingInformation(user.Result.StripCustomerId, body);
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        public async Task<IActionResult> ValidateProductSubscription(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                string userProductId = context.Items[Constants.TokenClaims.SubscriptionPublicKey]?.ToString();
                if (!string.IsNullOrEmpty(userProductId))
                {
                    SHA512DigitalSignatureWrapper dsWrapper = new SHA512DigitalSignatureWrapper();
                    User activeUser = await this._userRepository.GetUserById(userId);
                    if (dsWrapper.VerifyED25519(activeUser.UserSubscriptionSettings.SubscriptionPublicKey, Encoding.UTF8.GetBytes(userProductId), activeUser.UserSubscriptionSettings.SubscriptionDigitalSignature))
                    {
                        result = new OkObjectResult(new { });
                    }
                    else
                    {
                        result = new UnauthorizedObjectResult(new { });
                    }
                }
                else
                {
                    result = new UnauthorizedObjectResult(new { });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
    }
}
