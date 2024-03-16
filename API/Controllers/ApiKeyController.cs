using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Validation.Attributes;

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
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("RegenerateApiKey")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        public async Task<IActionResult> RegenerateApiKey()
        {
            return await this._controllerLogic.RegenerateApiKey(this._httpContextAccessor.HttpContext);
        }
    }
}
