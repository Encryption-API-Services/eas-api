using Microsoft.AspNetCore.Mvc;

namespace API.ControllerLogic
{
    public interface IUserAdminControllerLogic
    {
        public Task<IActionResult> GetUsers(HttpContext httpContext, int pageSkip, int pageSize);
    }
}
