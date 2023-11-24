using Microsoft.AspNetCore.Mvc;
using Models.Encryption.ED25519;

namespace API.ControllerLogic
{
    public interface IED25519ControllerLogic
    {
        Task<IActionResult> GetED25519KeyPair(HttpContext httpContext);
        Task<IActionResult> SignDataWithKeyPair(HttpContext httpContext, ED25519SignWithKeyPairRequest body);
        Task<IActionResult> VerifyDataWithPublicKey(HttpContext httpContext, Ed25519VerifyWithPublicKeyRequest body);
    }
}
