using Microsoft.AspNetCore.Mvc;

namespace API.ControllerLogic
{
    public interface IApiKeyControllerLogic
    {
        public Task<IActionResult> RegenerateApiKey(HttpContext httpContext);
    }
}
