using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.TwoFactorAuthentication;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TwoFAController : ControllerBase
    {
        private readonly ITwoFAControllerLogic _twoFAControllerLogic;
        public TwoFAController(ITwoFAControllerLogic twoFAControllerLogic)
        {
            this._twoFAControllerLogic = twoFAControllerLogic;
        }

        [HttpGet]
        [Route("Get2FAStatus")]
        [ValidateJWT]
        public async Task<IActionResult> Get2FAStatus()
        {
            return await this._twoFAControllerLogic.Get2FAStatus(HttpContext);
        }

        [HttpPut]
        [Route("TurnOn2FA")]
        [ValidateJWT]
        public async Task<IActionResult> TurnOn2FA()
        {
            return await this._twoFAControllerLogic.TurnOn2FA(HttpContext);
        }

        [HttpPut]
        [Route("TurnOff2FA")]
        [ValidateJWT]
        public async Task<IActionResult> TurnOff2FA()
        {
            return await this._twoFAControllerLogic.TurnOff2FA(HttpContext);
        }

        [HttpPut]
        [Route("UpdatePhoneNumber")]
        [ValidateJWT]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumber body)
        {
            return await this._twoFAControllerLogic.PhoneNumberUpdate(body, HttpContext);
        }
    }
}
