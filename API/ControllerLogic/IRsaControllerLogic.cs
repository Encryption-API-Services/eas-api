using Microsoft.AspNetCore.Mvc;
using Models.Encryption;

namespace API.ControllerLogic
{
    public interface IRsaControllerLogic
    {
        Task<IActionResult> GetKeyPair(HttpContext context, int keySize);
        Task<IActionResult> EncryptWithoutPublic(HttpContext context, RsaEncryptWithoutPublicRequest body);
        Task<IActionResult> EncryptWithPublic(HttpContext context, EncryptWithPublicRequest body);
        Task<IActionResult> DecryptWithStoredPrivate(HttpContext context, RsaDecryptWithStoredPrivateRequest body);
        Task<IActionResult> Decrypt(HttpContext context, RsaDecryptRequest body);
        Task<IActionResult> SignWithoutKey(HttpContext context, RsaSignWithoutKeyRequest body);
        Task<IActionResult> Verify(HttpContext context, RsaVerifyRequest body);
        Task<IActionResult> SignWithKey(HttpContext context, RsaSignWithKeyRequest body);
    }
}