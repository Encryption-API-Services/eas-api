using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;

namespace API.ControllersLogic
{
    public interface IUserRegisterControllerLogic
    {
        public Task<IActionResult> RegisterUser(RegisterUser body, HttpContext context);
        public Task<IActionResult> ActivateUser(ActivateUser body, HttpContext context);
        public Task<IActionResult> InactiveUser(InactiveUser body, HttpContext context);
        public Task<IActionResult> DeleteUser(HttpContext context);
    }
}
