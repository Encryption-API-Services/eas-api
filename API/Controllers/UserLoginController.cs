using API.ControllersLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserLoginControllerLogic _loginControllerLogic;

        public UserLoginController(IUserLoginControllerLogic loginControllerLogic)
        {
            this._loginControllerLogic = loginControllerLogic;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        // POST: UserLoginController
        public async Task<IActionResult> LoginUser([FromBody] LoginUser body)
        {
            return await this._loginControllerLogic.LoginUser(body, HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [Route("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody] UnlockUser body)
        {
            return await this._loginControllerLogic.UnlockUser(body, HttpContext);
        }

        [HttpPut]
        [Route("ValidateHotpCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateHotpCode([FromBody] ValidateHotpCode body)
        {
            return await this._loginControllerLogic.ValidateHotpCode(body, HttpContext);
        }

        [HttpGet]
        [Route("GetSuccessfulLogins")]
        [ValidateJWT]
        public async Task<IActionResult> GetSuccessfulLogins([FromQuery] int pageSkip, [FromQuery] int pageSize)
        {
            return await this._loginControllerLogic.GetSuccessfulLogins(HttpContext, pageSkip, pageSize);
        }

        [HttpPost]
        [Route("WasLoginMe")]
        [ValidateJWT]
        public async Task<IActionResult> WasLoginMe([FromBody] WasLoginMe body)
        {
            return await this._loginControllerLogic.WasLoginMe(body, HttpContext);
        }

        [HttpGet]
        [Route("GetApiKey")]
        [ValidateJWT]
        public async Task<IActionResult> GetApiKey()
        {
            return await this._loginControllerLogic.GetApiKey(HttpContext);
        }
    }
}