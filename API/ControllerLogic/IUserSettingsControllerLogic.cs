using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;
using Models.UserSettings;

namespace API.ControllerLogic
{
    public interface IUserSettingsControllerLogic
    {
        public Task<IActionResult> ChangeUsername(HttpContext context, ChangeUserName changeUsername);
        public Task<IActionResult> ChangePassword(HttpContext context, ChangePassword changePassword);
        public Task<IActionResult> Change2FAStatus(HttpContext context, Change2FAStatusRequest changePassword);
    }
}
