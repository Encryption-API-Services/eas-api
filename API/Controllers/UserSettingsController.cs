using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;
using Models.UserSettings;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserSettingsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserSettingsControllerLogic _userSettingsControllerLogic;
        public UserSettingsController(
            IHttpContextAccessor httpContextAccessor,
            IUserSettingsControllerLogic userSettingsControllerLogic
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userSettingsControllerLogic = userSettingsControllerLogic;
        }

        [HttpPut]
        [Route("Username")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUserName body)
        {
            return await this._userSettingsControllerLogic.ChangeUsername(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPut]
        [Route("Password")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword body)
        {
            return await this._userSettingsControllerLogic.ChangePassword(this._httpContextAccessor.HttpContext, body);
        }
    }
}
