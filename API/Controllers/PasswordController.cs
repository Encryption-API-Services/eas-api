using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.UserAuthentication;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordControllerLogic _passwordControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PasswordController(
            IPasswordControllerLogic passwordControllerLogic,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._passwordControllerLogic = passwordControllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("BCryptEncrypt")]
        public async Task<IActionResult> BcryptPassword([FromBody] BCryptEncryptModel body)
        {
            return await this._passwordControllerLogic.BcryptEncryptPassword(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("BcryptEncryptBatch")]
        public async Task<IActionResult> BcryptEncryptBatch([FromBody] BCryptEncryptBatchRequest body)
        {
            return await this._passwordControllerLogic.BcryptEncryptBatch(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("BcryptVerify")]
        public async Task<IActionResult> BcryptVerifyPassword([FromBody] BcryptVerifyModel body)
        {
            return await this._passwordControllerLogic.BcryptVerifyPassword(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest body)
        {
            return await this._passwordControllerLogic.ForgotPassword(body, this._httpContextAccessor.HttpContext);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest body)
        {
            return await this._passwordControllerLogic.ResetPassword(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("SCryptEncrypt")]
        public async Task<IActionResult> SCryptEncrypt([FromBody] ScryptHashRequest body)
        {
            return await this._passwordControllerLogic.ScryptHashPassword(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("SCryptEncryptBatch")]
        public async Task<IActionResult> SCryptEncryptBatch([FromBody] SCryptEncryptBatchRequest body)
        {
            return await this._passwordControllerLogic.SCryptEncryptBatch(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("SCryptVerify")]
        public async Task<IActionResult> SCryptVerify([FromBody] SCryptVerifyRequest body)
        {
            return await this._passwordControllerLogic.ScryptVerifyPassword(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("Argon2Hash")]
        public async Task<IActionResult> Argon2Hash([FromBody] Argon2HashRequest body)
        {
            return await this._passwordControllerLogic.Argon2Hash(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("Argon2HashBatch")]
        public async Task<IActionResult> Argon2HashBatch([FromBody] Argon2HashBatchRequest body)
        {
            return await this._passwordControllerLogic.Argon2HashBatch(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("Argon2Verify")]
        public async Task<IActionResult> Argon2Verify([FromBody] Argon2VerifyRequest body)
        {
            return await this._passwordControllerLogic.Argon2Verify(body, this._httpContextAccessor.HttpContext);
        }
    }
}