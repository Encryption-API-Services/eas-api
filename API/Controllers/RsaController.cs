using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RsaController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRsaControllerLogic _rsaControllerLogic;
        public RsaController(
            IHttpContextAccessor httpContextAccessor,
            IRsaControllerLogic rsaContollerLogic
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._rsaControllerLogic = rsaContollerLogic;
        }

        [HttpGet]
        [Route("GetKeyPair")]
        public async Task<IActionResult> GetKeyPair([FromQuery] int keySize)
        {
            return await this._rsaControllerLogic.GetKeyPair(this._httpContextAccessor.HttpContext, keySize);
        }

        [HttpPost]
        [Route("EncryptWithoutPublic")]
        public async Task<IActionResult> EncryptWithoutPublic([FromBody] RsaEncryptWithoutPublicRequest body)
        {
            return await this._rsaControllerLogic.EncryptWithoutPublic(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("EncryptWithPublic")]
        public async Task<IActionResult> EncryptWithPublic([FromBody] EncryptWithPublicRequest body)
        {
            return await this._rsaControllerLogic.EncryptWithPublic(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("Decrypt")]
        public async Task<IActionResult> Decrypt([FromBody] RsaDecryptRequest body)
        {
            return await this._rsaControllerLogic.Decrypt(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("DecryptWithStoredPrivate")]
        public async Task<IActionResult> DecryptWithStoredPrivate([FromBody] RsaDecryptWithStoredPrivateRequest body)
        {
            return await this._rsaControllerLogic.DecryptWithStoredPrivate(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("SignWithoutKey")]
        public async Task<IActionResult> SignWithoutKey([FromBody] RsaSignWithoutKeyRequest body)
        {
            return await this._rsaControllerLogic.SignWithoutKey(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("SignWithKey")]
        public async Task<IActionResult> SignWithKey([FromBody] RsaSignWithKeyRequest body)
        {
            return await this._rsaControllerLogic.SignWithKey(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost]
        [Route("Verify")]
        public async Task<IActionResult> Verify([FromBody] RsaVerifyRequest body)
        {
            return await this._rsaControllerLogic.Verify(this._httpContextAccessor.HttpContext, body);
        }
    }
}