using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly IApiKeyControllerLogic _controllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiKeyController(
            IApiKeyControllerLogic controllerLogic, 
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._controllerLogic = controllerLogic;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("RegenerateApiKey")]
        public async Task<IActionResult> RegenerateApiKey()
        {
            return await this._controllerLogic.RegenerateApiKey(this._httpContextAccessor.HttpContext);
        }
    }
}
