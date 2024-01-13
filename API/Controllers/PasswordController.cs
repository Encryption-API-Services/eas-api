using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.UserAuthentication;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordControllerLogic _passwordControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PasswordController(
            IPasswordControllerLogic passwordControllerLogic,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._passwordControllerLogic = passwordControllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest body)
        {
            return await this._passwordControllerLogic.ForgotPassword(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest body)
        {
            return await this._passwordControllerLogic.ResetPassword(body, this._httpContextAccessor.HttpContext);
        }
    }
}
