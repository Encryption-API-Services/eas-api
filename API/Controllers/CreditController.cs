using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Credit;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly ICreditControllerLogic _creditControllerLogic;

        public CreditController(ICreditControllerLogic creditControllerLogic)
        {
            this._creditControllerLogic = creditControllerLogic;
        }

        [HttpPost]
        [Route("ValidateCard")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> ValidateCreditCard([FromBody] CreditValidateRequest body)
        {
            return await this._creditControllerLogic.ValidateCreditCard(body, HttpContext);
        }

        [HttpPost]
        [Route("AddCreditCard")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> AddCreditCard([FromBody] AddCreditCardRequest body)
        {
            return await this._creditControllerLogic.AddCreditCard(body, HttpContext);
        }
    }
}