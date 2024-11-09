using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.EmergencyKit;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmergencyKitController : ControllerBase
    {
        private readonly IEmergencyKitControllerLogic _controllerLogic;
        private readonly IHttpContextAccessor _contextAccessor;

        public EmergencyKitController(
            IHttpContextAccessor contextAccessor,
            IEmergencyKitControllerLogic controllerLogic)
        {
            this._contextAccessor = contextAccessor;    
            this._controllerLogic = controllerLogic;
        }

        [HttpPost]
        [Route("RecoverProfile")]
        public async Task<IActionResult> RecoverProfile([FromBody] EmgerencyKitRecoverProfileRequest request)
        {
            return await this._controllerLogic.RecoverProfile(this._contextAccessor.HttpContext, request);
        }
    }
}