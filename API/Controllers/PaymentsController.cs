using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Payments;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentsControllerLogic _paymentsControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaymentsController(
            IPaymentsControllerLogic paymentsControllerLogic,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._paymentsControllerLogic = paymentsControllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("CreateProduct")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestBody body)
        {
            return await this._paymentsControllerLogic.CreateProduct(this._httpContextAccessor.HttpContext, body);
        }

        [HttpGet]
        [Route("GetProducts")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> GetProducts()
        {
            return await this._paymentsControllerLogic.GetProducts(this._httpContextAccessor.HttpContext);
        }

        [HttpGet]
        [Route("GetProductsWithPrice")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> GetProductsWithPrice()
        {
            return await this._paymentsControllerLogic.GetProductsWithPrice(this._httpContextAccessor.HttpContext);
        }

        [HttpPut]
        [Route("AssignProductToUser")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> AssignProductToUser([FromBody] AssignProductToUserRequestBody body)
        {
            return await this._paymentsControllerLogic.AssignProductToUser(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("CreatePrice")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> CreatePrice([FromBody] CreatePriceRequestBody body)
        {
            return await this._paymentsControllerLogic.CreatePrice(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPut]
        [Route("DisableSubscription")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> DisableSubscription()
        {
            return await this._paymentsControllerLogic.DisableSubscription(this._httpContextAccessor.HttpContext);
        }

        [HttpGet]
        [Route("GetBillingInformation")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> GetBillingInformation()
        {
            return await this._paymentsControllerLogic.GetBillingInformation(this._httpContextAccessor.HttpContext);
        }

        [HttpPut]
        [Route("UpdateBillingInformation")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> UpdateBillingInformation([FromBody] UpdateBillingInformationRequestBody body)
        {
            return await this._paymentsControllerLogic.UpdateBillingInformation(this._httpContextAccessor.HttpContext, body);
        }
    }
}
