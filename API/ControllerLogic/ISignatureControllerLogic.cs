using Microsoft.AspNetCore.Mvc;
using Models.Encryption.Signatures;

namespace API.ControllerLogic
{
    public interface ISignatureControllerLogic
    {
        Task<IActionResult> SHA512SignedRsa(SHA512SignedRSARequest body, HttpContext httpContext);
        Task<IActionResult> SHA512SignedRsaVerify(SHA512SignedRSAVerifyRequest body, HttpContext httpContext);
        Task<IActionResult> SHA512ED25519DalekSign(SHA512ED25519DalekSignRequest body, HttpContext httpContext);
        Task<IActionResult> SHA512ED25519DalekVerify(SHA512ED25519DalekVerifyRequest body, HttpContext httpContext);
        Task<IActionResult> HMACSign(HMACSignRequest body, HttpContext httpContext);
        Task<IActionResult> HMACVerify(HMACVerifyRequest body, HttpContext httpContext);
        Task<IActionResult> Blake2RsaSign(Blake2RSASignRequest body, HttpContext httpContext);
        Task<IActionResult> Blake2RsaVerify(Blake2RSAVerifyRequest body, HttpContext httpContext);
        Task<IActionResult> Blake2ED25519DalekSign(Blake2ED25519DalekSignRequest body, HttpContext httpContext);
        Task<IActionResult> Blake2ED25519DalekVerify([FromBody] Blake2ED25519DalekVerifyRequest body, HttpContext httpContext);
    }
}
