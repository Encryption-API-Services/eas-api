using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserAdminController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserAdminControllerLogic _userAdminControllerLogic;
        public UserAdminController(
            IUserAdminControllerLogic userAdminControllerLogic, 
            IHttpContextAccessor contextAccessor)
        {
            this._userAdminControllerLogic = userAdminControllerLogic;
            this._contextAccessor = contextAccessor;
        }

        [HttpGet]
        [Route("GetUsers")]
        [ValidateJWT]
        public async Task<IActionResult> GetUsers()
        {
            return await this._userAdminControllerLogic.GetUsers(this._contextAccessor.HttpContext);
        }
    }
}
