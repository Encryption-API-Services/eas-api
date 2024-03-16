using Microsoft.AspNetCore.Mvc;
using Models.UserAdmin;

namespace API.ControllerLogic
{
    public interface IUserAdminControllerLogic
    {
        public Task<IActionResult> GetUsers(HttpContext httpContext, int pageSkip, int pageSize);
        public Task<IActionResult> UserActivationStatus(HttpContext httpContext, UserActivationStatusRequest request);
        public Task<IActionResult> UserAdminStatus(HttpContext context, UserAdminStatusRequest request);
        public Task<IActionResult> DeleteUser(HttpContext context, UserAdminDeleteUserRequest request);
    }
}
