using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;

namespace API.ControllerLogic
{
    public interface IUserSettingsControllerLogic
    {
        public Task<IActionResult> ChangeUsername(HttpContext context, ChangeUserName changeUsername);
        public Task<IActionResult> ChangePassword(HttpContext context, ChangePassword changePassword);
    }
}
