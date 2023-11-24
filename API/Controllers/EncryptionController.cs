using API.ControllersLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.Encryption.AESRSAHybrid;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        private readonly IEncryptionControllerLogic _encryptionControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EncryptionController(
            IEncryptionControllerLogic controllerLogic,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._encryptionControllerLogic = controllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("EncryptAES")]
        public async Task<IActionResult> EncryptAES([FromBody] EncryptAESRequest body)
        {
            return await this._encryptionControllerLogic.EncryptAES(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("DecryptAES")]
        public async Task<IActionResult> DecryptAES([FromBody] DecryptAESRequest body)
        {
            return await this._encryptionControllerLogic.DecryptAES(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("EncryptSHA512")]
        public async Task<IActionResult> EncryptSHA512([FromBody] EncryptSHARequest body)
        {
            return await this._encryptionControllerLogic.EncryptSHA512(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("EncryptSHA256")]
        public async Task<IActionResult> EncryptSHA256([FromBody] EncryptSHARequest body)
        {
            return await this._encryptionControllerLogic.EncryptSHA256(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("HashMD5")]
        public async Task<IActionResult> HashMD5([FromBody] MD5Request body)
        {
            return await this._encryptionControllerLogic.HashMD5(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("VerifyMD5")]
        public async Task<IActionResult> VerifyMD5([FromBody] MD5VerifyRequest body)
        {
            return await this._encryptionControllerLogic.VerifyMD5(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("EncryptAESRSAHybrid")]
        public async Task<IActionResult> EncryptAESRSAHybrid([FromBody] AESRSAHybridEncryptRequest body)
        {
            return await this._encryptionControllerLogic.EncryptAESRSAHybrid(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("DecryptAESRSAHybrid")]
        public async Task<IActionResult> DecryptAESRSAHybrid([FromBody] AESRSAHybridDecryptRequest body)
        {
            return await this._encryptionControllerLogic.DecryptAESRSAHybrid(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("Blake2")]
        public async Task<IActionResult> Blake2Hash([FromBody] Blake2Request body)
        {
            return await this._encryptionControllerLogic.Blake2Hash(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost]
        [Route("Blake2Verify")]
        public async Task<IActionResult> Blake2Verify([FromBody] Blake2VerifyRequest body)
        {
            return await this._encryptionControllerLogic.Blake2Verify(body, this._httpContextAccessor.HttpContext);
        }
    }
}