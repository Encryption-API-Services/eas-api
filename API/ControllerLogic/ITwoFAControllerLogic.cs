using Microsoft.AspNetCore.Mvc;
using Models.TwoFactorAuthentication;

namespace API.ControllersLogic
{
    public interface ITwoFAControllerLogic
    {
        public Task<IActionResult> Get2FAStatus(HttpContext httpContext);
        public Task<IActionResult> TurnOn2FA(HttpContext httpContext);
        public Task<IActionResult> TurnOff2FA(HttpContext httpContext);
        public Task<IActionResult> PhoneNumberUpdate(UpdatePhoneNumber body, HttpContext httpContext);
    }
}
