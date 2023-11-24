using API.ControllerLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.Signatures;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly ISignatureControllerLogic _signatureControllerLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SignatureController(
            ISignatureControllerLogic signatureControllerLogic, IHttpContextAccessor httpContextAccessor)
        {
            this._signatureControllerLogic = signatureControllerLogic;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("SHA512SignedRSA")]
        public async Task<IActionResult> SHA512SignedRSA([FromBody] SHA512SignedRSARequest body)
        {
            return await this._signatureControllerLogic.SHA512SignedRsa(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("SHA512SignedRSAVerify")]
        public async Task<IActionResult> SHA512SignedRSAVerify([FromBody] SHA512SignedRSAVerifyRequest body)
        {
            return await this._signatureControllerLogic.SHA512SignedRsaVerify(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("SHA512ED25519DalekSign")]
        public async Task<IActionResult> SHA512ED25519DalekSign([FromBody] SHA512ED25519DalekSignRequest body)
        {
            return await this._signatureControllerLogic.SHA512ED25519DalekSign(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("SHA512ED25519DalekVerify")]
        public async Task<IActionResult> SHA512ED25519DalekVerify([FromBody] SHA512ED25519DalekVerifyRequest body)
        {
            return await this._signatureControllerLogic.SHA512ED25519DalekVerify(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("HMACSign")]
        public async Task<IActionResult> HMACSign([FromBody] HMACSignRequest body)
        {
            return await this._signatureControllerLogic.HMACSign(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("HMACVerify")]
        public async Task<IActionResult> HMACVerify([FromBody] HMACVerifyRequest body)
        {
            return await this._signatureControllerLogic.HMACVerify(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("Blake2RsaSign")]
        public async Task<IActionResult> Blake2RsaSign([FromBody] Blake2RSASignRequest body)
        {
            return await this._signatureControllerLogic.Blake2RsaSign(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("Blake2RsaVerify")]
        public async Task<IActionResult> Blake2RsaVerify([FromBody] Blake2RSAVerifyRequest body)
        {
            return await this._signatureControllerLogic.Blake2RsaVerify(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("Blake2ED25519DalekSign")]
        public async Task<IActionResult> Blake2ED25519DalekSign([FromBody] Blake2ED25519DalekSignRequest body)
        {
            return await this._signatureControllerLogic.Blake2ED25519DalekSign(body, this._httpContextAccessor.HttpContext);
        }

        [HttpPost("Blake2ED25519DalekVerify")]
        public async Task<IActionResult> Blake2ED25519DalekVerify([FromBody] Blake2ED25519DalekVerifyRequest body)
        {
            return await this._signatureControllerLogic.Blake2ED25519DalekVerify(body, this._httpContextAccessor.HttpContext);
        }
    }
}