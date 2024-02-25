using CASHelpers.Types.HttpResponses.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication.AuthenticationController;

namespace API.ControllerLogic
{
    public interface IAuthenticationControllerLogic
    {
        public Task<IActionResult> StoreOperatingSystemInformationInCache(HttpContext httpContext, OperatingSystemInformationCacheRequestBody body);
        public Task<IActionResult> RemoveOperatingSystemInformationInCache(HttpContext httpContext);
        public Task<IActionResult> DiffieHellmanAesKeyDerviationForSDK(HttpContext httpContext, DiffieHellmanAesDerivationRequest body);
    }
}
