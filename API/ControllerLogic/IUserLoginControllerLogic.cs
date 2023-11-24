using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;

namespace API.ControllersLogic
{
    public interface IUserLoginControllerLogic
    {
        public Task<IActionResult> LoginUser(LoginUser body, HttpContext httpContext);
        public Task<IActionResult> UnlockUser(UnlockUser body, HttpContext context);
        public Task<IActionResult> ValidateHotpCode([FromBody] ValidateHotpCode body, HttpContext context);
        public Task<IActionResult> GetSuccessfulLogins(HttpContext context, int pageSkip, int pageSize);
        public Task<IActionResult> WasLoginMe(WasLoginMe body, HttpContext context);
        public Task<IActionResult> GetApiKey(HttpContext context);
    }
}