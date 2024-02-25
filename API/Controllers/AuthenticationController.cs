using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication.AuthenticationController;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationControllerLogic _authenicationControllerLogic;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AuthenticationController(
            IAuthenticationControllerLogic authenicationControllerLogic,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._authenicationControllerLogic = authenicationControllerLogic;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("OperatingSystemCacheStore")]
        [ValidateJWT]
        public async Task<IActionResult> StoreOperatingSystemInformationInCache([FromBody] OperatingSystemInformationCacheRequestBody body)
        {
            return await this._authenicationControllerLogic.StoreOperatingSystemInformationInCache(this.httpContextAccessor.HttpContext, body);
        }

        [HttpPut]
        [Route("OperatingSystemCacheRemove")]
        [ValidateJWT]
        public async Task<IActionResult> RemoveOperatingSystemInformationInCache()
        {
            return await this._authenicationControllerLogic.RemoveOperatingSystemInformationInCache(this.httpContextAccessor.HttpContext);
        }
    }
}
