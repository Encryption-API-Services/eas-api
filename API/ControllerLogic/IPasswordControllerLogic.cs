using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.UserAuthentication;

namespace API.ControllersLogic
{
    public interface IPasswordControllerLogic
    {
        Task<IActionResult> BcryptEncryptPassword(BCryptEncryptModel body, HttpContext context);
        Task<IActionResult> BcryptEncryptBatch(BCryptEncryptBatchRequest body, HttpContext context);
        Task<IActionResult> BcryptVerifyPassword(BcryptVerifyModel body, HttpContext context);
        Task<IActionResult> ForgotPassword(ForgotPasswordRequest email, HttpContext context);
        Task<IActionResult> ResetPassword(ResetPasswordRequest body, HttpContext context);
        Task<IActionResult> ScryptHashPassword(ScryptHashRequest body, HttpContext context);
        Task<IActionResult> SCryptEncryptBatch(SCryptEncryptBatchRequest body, HttpContext context);
        Task<IActionResult> ScryptVerifyPassword(SCryptVerifyRequest body, HttpContext context);
        Task<IActionResult> Argon2Hash(Argon2HashRequest body, HttpContext context);
        Task<IActionResult> Argon2HashBatch(Argon2HashBatchRequest body, HttpContext context);
        Task<IActionResult> Argon2Verify(Argon2VerifyRequest body, HttpContext context);
    }
}
