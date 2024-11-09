using Microsoft.AspNetCore.Mvc;
using Models.EmergencyKit;

namespace API.ControllerLogic
{
    public interface IEmergencyKitControllerLogic
    {
        public Task<IActionResult> RecoverProfile(HttpContext context, EmgerencyKitRecoverProfileRequest request);
    }
}
