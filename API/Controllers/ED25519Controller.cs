using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.ED25519;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ED25519Controller : ControllerBase
    {
        private readonly IED25519ControllerLogic _ed25519ControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ED25519Controller(
            IED25519ControllerLogic eD25519ControllerLogic,
            IHttpContextAccessor httpContextAccessor)
        {
            this._ed25519ControllerLogic = eD25519ControllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("KeyPair")]
        public async Task<IActionResult> GetED25519KeyPair()
        {
            return await this._ed25519ControllerLogic.GetED25519KeyPair(this._httpContextAccessor.HttpContext);
        }

        [HttpPost("SignWithKeyPair")]
        public async Task<IActionResult> SignWithKeyPair(ED25519SignWithKeyPairRequest body)
        {
            return await this._ed25519ControllerLogic.SignDataWithKeyPair(this._httpContextAccessor.HttpContext, body);
        }

        [HttpPost("VerifyWithPublicKey")]
        public async Task<IActionResult> VerifyWithPublicKey(Ed25519VerifyWithPublicKeyRequest body)
        {
            return await this._ed25519ControllerLogic.VerifyDataWithPublicKey(this._httpContextAccessor.HttpContext, body);
        }
    }
}