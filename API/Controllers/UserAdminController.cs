using API.ControllerLogic;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Models.UserAdmin;
using Validation.Attributes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserAdminController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserAdminControllerLogic _userAdminControllerLogic;
        private readonly IRedisClient _redisClient;
        public UserAdminController(
            IUserAdminControllerLogic userAdminControllerLogic,
            IHttpContextAccessor contextAccessor,
            IRedisClient redisClient
            )
        {
            this._userAdminControllerLogic = userAdminControllerLogic;
            this._contextAccessor = contextAccessor;
            this._redisClient = redisClient;
        }

        [HttpGet]
        [Route("GetUsers")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        [TypeFilter(typeof(IsAdminAttribute))]
        public async Task<IActionResult> GetUsers([FromQuery] int pageSkip, [FromQuery] int pageSize)
        {
            return await this._userAdminControllerLogic.GetUsers(this._contextAccessor.HttpContext, pageSkip, pageSize);
        }

        [HttpPut]
        [Route("UserActivationStatus")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        [TypeFilter(typeof(IsAdminAttribute))]
        public async Task<IActionResult> UserActivationStatus([FromBody] UserActivationStatusRequest request)
        {
            return await this._userAdminControllerLogic.UserActivationStatus(this._contextAccessor.HttpContext, request);
        }

        [HttpPut]
        [Route("UserAdminStatus")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        [TypeFilter(typeof(IsAdminAttribute))]
        public async Task<IActionResult> UserAdminStatus([FromBody] UserAdminStatusRequest request)
        {
            return await this._userAdminControllerLogic.UserAdminStatus(this._contextAccessor.HttpContext, request);
        }

        [HttpDelete]
        [Route("UserDelete")]
        [TypeFilter(typeof(ValidateJWTAttribute))]
        [TypeFilter(typeof(IsAdminAttribute))]
        public async Task<IActionResult> DeleteUser([FromBody] UserAdminDeleteUserRequest request) 
        {
            return await this._userAdminControllerLogic.DeleteUser(this._contextAccessor.HttpContext, request);
        }
    }
}
