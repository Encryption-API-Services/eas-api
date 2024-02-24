using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Validation.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenControllerLogic _tokenControllerLogic;
        public TokenController(
            IHttpContextAccessor httpContextAccessor,
            ITokenControllerLogic tokenControllerLogic)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._tokenControllerLogic = tokenControllerLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            return await this._tokenControllerLogic.GetToken(this._httpContextAccessor.HttpContext);
        }

        [HttpGet]
        [Route("RefreshToken")]
        public async Task<IActionResult> GetRefreshToken()
        {
            return await this._tokenControllerLogic.GetRefreshToken(this._httpContextAccessor.HttpContext);
        }

        [HttpGet]
        [Route("IsTokenValid")]
        public async Task<IActionResult> IsTokenValid()
        {
            return await this._tokenControllerLogic.IsTokenValid(this._httpContextAccessor.HttpContext);
        }
    }
}