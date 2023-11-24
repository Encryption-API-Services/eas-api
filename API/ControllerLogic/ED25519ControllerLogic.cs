using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Encryption;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.ED25519;
using System.Reflection;
using System.Runtime.InteropServices;
using static Encryption.ED25519Wrapper;

namespace API.ControllerLogic
{
    public class ED25519ControllerLogic : IED25519ControllerLogic
    {
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;

        public ED25519ControllerLogic(
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache methodBenchmarkCache)
        {
            this._exceptionRepository = exceptionRepository;
            this._benchmarkMethodCache = methodBenchmarkCache;
        }
        public async Task<IActionResult> GetED25519KeyPair(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                ED25519Wrapper ed25519 = new ED25519Wrapper();
                IntPtr keyPairPtr = await ed25519.GetKeyPairAsync();
                string keyPair = Marshal.PtrToStringAnsi(keyPairPtr);
                ED25519Wrapper.free_cstring(keyPairPtr);
                result = new OkObjectResult(new ED25519GetKeyPairResponse() { KeyPair = keyPair });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "Some went wrong getting your ED25519 keys on our end" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> SignDataWithKeyPair(HttpContext httpContext, ED25519SignWithKeyPairRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                ED25519Wrapper ed25519 = new ED25519Wrapper();
                Ed25519SignatureResult signResult = await ed25519.SignAsync(body.KeyPair, body.DataToSign);
                string publicKey = Marshal.PtrToStringAnsi(signResult.Public_Key);
                string signature = Marshal.PtrToStringAnsi(signResult.Signature);
                ED25519Wrapper.free_cstring(signResult.Public_Key);
                ED25519Wrapper.free_cstring(signResult.Signature);
                result = new OkObjectResult(new ED25519SignDataResponse() { PublicKey = publicKey, Signature = signature });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "Some went wrong signing your data on our end" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> VerifyDataWithPublicKey(HttpContext httpContext, Ed25519VerifyWithPublicKeyRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                ED25519Wrapper ed25519 = new ED25519Wrapper();
                bool isValid = await ed25519.VerifyWithPublicAsync(body.PublicKey, body.Signature, body.DataToVerify);
                result = new OkObjectResult(new ED25519VerifyResponse() { IsValid = isValid });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "Some went wrong verifying your data on our end" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
    }
}