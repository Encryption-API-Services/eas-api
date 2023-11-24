using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.Encryption.AESRSAHybrid;

namespace API.ControllersLogic
{
    public interface IEncryptionControllerLogic
    {
        public Task<IActionResult> EncryptAES(EncryptAESRequest body, HttpContext httpContext);
        public Task<IActionResult> DecryptAES(DecryptAESRequest body, HttpContext httpContext);
        public Task<IActionResult> EncryptSHA512(EncryptSHARequest body, HttpContext httpContext);
        public Task<IActionResult> EncryptSHA256(EncryptSHARequest body, HttpContext httpContext);
        public Task<IActionResult> HashMD5(MD5Request body, HttpContext httpContext);
        public Task<IActionResult> VerifyMD5(MD5VerifyRequest body, HttpContext httpContext);
        public Task<IActionResult> EncryptAESRSAHybrid(AESRSAHybridEncryptRequest body, HttpContext httpContext);
        public Task<IActionResult> DecryptAESRSAHybrid(AESRSAHybridDecryptRequest body, HttpContext httpContext);
        public Task<IActionResult> Blake2Hash(Blake2Request body, HttpContext httpContext);
        public Task<IActionResult> Blake2Verify([FromBody] Blake2VerifyRequest body, HttpContext httpContext);
    }
}
