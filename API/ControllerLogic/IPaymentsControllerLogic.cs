using Microsoft.AspNetCore.Mvc;
using Models.Payments;

namespace API.ControllerLogic
{
    public interface IPaymentsControllerLogic
    {
        public Task<IActionResult> CreateProduct(HttpContext context, CreateProductRequestBody body);
        public Task<IActionResult> GetProducts(HttpContext context);
        public Task<IActionResult> GetProductsWithPrice(HttpContext context);
        public Task<IActionResult> CreatePrice(HttpContext context, CreatePriceRequestBody body);
        public Task<IActionResult> AssignProductToUser(HttpContext context, AssignProductToUserRequestBody body);
        public Task<IActionResult> DisableSubscription(HttpContext context);
        public Task<IActionResult> GetBillingInformation(HttpContext context);
        public Task<IActionResult> UpdateBillingInformation(HttpContext context, UpdateBillingInformationRequestBody body);
    }
}
