using Microsoft.AspNetCore.Mvc;
using Models.Credit;

namespace API.ControllersLogic
{
    public interface ICreditControllerLogic
    {
        public Task<IActionResult> ValidateCreditCard([FromBody] CreditValidateRequest body, HttpContext httpContext);
        public Task<IActionResult> AddCreditCard(AddCreditCardRequest body, HttpContext httpContext);
    }
}