using Microsoft.AspNetCore.Mvc;

namespace API.ControllerLogic
{
    public interface ITokenControllerLogic
    {
        public Task<IActionResult> GetToken(HttpContext httpContext);
        public Task<IActionResult> GetRefreshToken(HttpContext httpContext);
        public Task<IActionResult> IsTokenValid(HttpContext httpContext);
    }
}