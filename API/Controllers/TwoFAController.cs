using API.ControllersLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.TwoFactorAuthentication;

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
        [AllowAnonymous]
        public async Task<IActionResult> Get2FAStatus()
        {
            return await this._twoFAControllerLogic.Get2FAStatus(HttpContext);
        }

        [HttpPut]
        [Route("TurnOn2FA")]
        [AllowAnonymous]
        public async Task<IActionResult> TurnOn2FA()
        {
            return await this._twoFAControllerLogic.TurnOn2FA(HttpContext);
        }

        [HttpPut]
        [Route("TurnOff2FA")]
        [AllowAnonymous]
        public async Task<IActionResult> TurnOff2FA()
        {
            return await this._twoFAControllerLogic.TurnOff2FA(HttpContext);
        }

        [HttpPut]
        [Route("UpdatePhoneNumber")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumber body)
        {
            return await this._twoFAControllerLogic.PhoneNumberUpdate(body, HttpContext);
        }
    }
}
