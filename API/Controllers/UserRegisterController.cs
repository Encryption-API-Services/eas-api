using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private IUserRegisterControllerLogic _userRegisterLogic { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRegisterController(
            IUserRegisterControllerLogic userRegisterLogic,
            IHttpContextAccessor httpContextAccessor)
        {
            this._userRegisterLogic = userRegisterLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterUser body)
        {
            return await this._userRegisterLogic.RegisterUser(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Activate")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ActivateUser body)
        {
            return await this._userRegisterLogic.ActivateUser(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("InactiveUser")]
        [HttpPost]
        public async Task<IActionResult> InactiveUser([FromBody] InactiveUser body)
        {
            return await this._userRegisterLogic.InactiveUser(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("DeleteUser")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            return await this._userRegisterLogic.DeleteUser(this._httpContextAccessor.HttpContext);
        }
    }
}
