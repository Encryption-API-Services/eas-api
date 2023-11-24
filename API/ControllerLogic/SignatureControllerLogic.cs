using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.Signatures;
using System.Reflection;
using System.Runtime.InteropServices;
using Validation.Keys;
using static Encryption.ED25519Wrapper;
using static Encryption.RustRSAWrapper;

namespace API.ControllerLogic
{
    public class SignatureControllerLogic : ISignatureControllerLogic
    {
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        public SignatureControllerLogic(
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkCache
            )
        {
            this._exceptionRepository = exceptionRepository;
            this._benchmarkMethodCache = benchmarkCache;
        }

        #region Blake2Rsa

        public async Task<IActionResult> Blake2ED25519DalekSign(Blake2ED25519DalekSignRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.HashSize != 256 && body.HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "Hash size not supported for Blake2 ED25519 Dalek Digital Signature" });
                }
                else if (string.IsNullOrEmpty(body.DataToSign))
                {
                    result = new BadRequestObjectResult(new { error = "Need to supply data to sign with Blake2 ED25519 Dalek Digital Signature" });
                }
                else
                {
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    ED25519Wrapper ed25519Wrapper = new ED25519Wrapper();
                    Task<IntPtr> hashPtr = this.GetBlake2HashType(body.DataToSign, body.HashSize, blake2Wrapper);
                    Task<IntPtr> keyPairPtr = ed25519Wrapper.GetKeyPairAsync();
                    await Task.WhenAll(hashPtr, keyPairPtr);
                    string hash = Marshal.PtrToStringAnsi(hashPtr.Result);
                    string keyPair = Marshal.PtrToStringAnsi(keyPairPtr.Result);
                    Ed25519SignatureResult signatureResult = await ed25519Wrapper.SignAsync(keyPair, hash);
                    string publicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key);
                    string signature = Marshal.PtrToStringAnsi(signatureResult.Signature);
                    result = new OkObjectResult(new Blake2ED25519DalekSignResponse() { Signature = signature, PublicKey = publicKey });
                    Blake2Wrapper.free_cstring(hashPtr.Result);
                    ED25519Wrapper.free_cstring(keyPairPtr.Result);
                    ED25519Wrapper.free_cstring(signatureResult.Public_Key);
                    ED25519Wrapper.free_cstring(signatureResult.Signature);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> Blake2ED25519DalekVerify([FromBody] Blake2ED25519DalekVerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { error = "Need a public key to verify for Blake2 ED25519 Digital Signature" });
                }
                else if (string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { error = "Need a signature to verify for Blake2 ED25519 Digital Signature" });
                }
                else if (string.IsNullOrEmpty(body.DataToVerify))
                {
                    result = new BadRequestObjectResult(new { error = "Need data to verify for Blake2 ED25519 Dalek Digital Signature" });
                }
                else if (body.HashSize != 256 && body.HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "Hash size is not suppoted for Blake2 ED25519 Dalek Digital Signature" });
                }
                else
                {
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    ED25519Wrapper ed25519Wrapper = new ED25519Wrapper();
                    IntPtr hashPtr = await this.GetBlake2HashType(body.DataToVerify, body.HashSize, blake2Wrapper);
                    string hash = Marshal.PtrToStringAnsi(hashPtr);
                    bool isValid = await ed25519Wrapper.VerifyWithPublicAsync(body.PublicKey, body.Signature, hash);
                    result = new OkObjectResult(new SHA512ED25519DalekVerifyResponse { IsValid = isValid });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> Blake2RsaSign(Blake2RSASignRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.Blake2HashSize != 256 && body.Blake2HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "The supplied Blake2 Hash size is not supported" });
                }
                else if (body.RsaKeySize != 1024 && body.RsaKeySize != 2048 && body.RsaKeySize != 4096)
                {
                    result = new BadRequestObjectResult(new { error = "The AES Key size is no supported for Blake2 RSA Digital Signature" });
                }
                else if (string.IsNullOrEmpty(body.DataToSign))
                {
                    result = new BadRequestObjectResult(new { error = "Must provide data to sign with Blake2 RSA Digital Signature" });
                }
                else
                {
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    Task<IntPtr> hashPtr = this.GetBlake2HashType(body.DataToSign, body.Blake2HashSize, blake2Wrapper);
                    Task<RustRsaKeyPair> keyPair = rsaWrapper.GetKeyPairAsync(body.RsaKeySize);
                    await Task.WhenAll(hashPtr, keyPair);
                    string hash = Marshal.PtrToStringAnsi(hashPtr.Result);
                    string privateKey = Marshal.PtrToStringAnsi(keyPair.Result.priv_key);
                    string publicKey = Marshal.PtrToStringAnsi(keyPair.Result.pub_key);
                    RSAValidator rsaValidator = new RSAValidator();
                    if (!rsaValidator.IsPublicKeyPEMValid(publicKey) || !rsaValidator.IsPrivateKeyPEMValid(privateKey))
                    {
                        result = new BadRequestObjectResult(new { error = "We experienced a problem creating your Blake2 RSA Digital Signature" });
                    }
                    else
                    {
                        IntPtr signaturePtr = await rsaWrapper.RsaSignWithKeyAsync(privateKey, hash);
                        string signature = Marshal.PtrToStringAnsi(signaturePtr);
                        result = new OkObjectResult(new Blake2RSASignResponse() { Signature = signature, PrivateKey = privateKey, PublicKey = publicKey });
                        RustRSAWrapper.free_cstring(signaturePtr);
                    }
                    Blake2Wrapper.free_cstring(hashPtr.Result);
                    RustRSAWrapper.free_cstring(keyPair.Result.pub_key);
                    RustRSAWrapper.free_cstring(keyPair.Result.priv_key);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "We experienced a problem creating your Blake2 RSA Digital Signature" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> Blake2RsaVerify([FromBody] Blake2RSAVerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.Blake2HashSize != 256 && body.Blake2HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "Blake2 Hash Size is not supported for digital signature verification" });
                }
                else if (string.IsNullOrEmpty(body.PublicKey) || !new RSAValidator().IsPublicKeyPEMValid(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide and RSA public key for digital signature verification" });
                }
                else if (string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a signature for digital signature verification" });
                }
                else if (string.IsNullOrEmpty(body.OriginalData))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to verify with digital signature verification" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    IntPtr originalHashPtr = await this.GetBlake2HashType(body.OriginalData, body.Blake2HashSize, blake2Wrapper);
                    string originalHash = Marshal.PtrToStringAnsi(originalHashPtr);
                    bool isValid = await rsaWrapper.RsaVerifyAsync(body.PublicKey, originalHash, body.Signature);
                    result = new OkObjectResult(new Blake2RSAVerifyResponse() { IsValid = isValid });
                    RustSHAWrapper.free_cstring(originalHashPtr);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        private async Task<IntPtr> GetBlake2HashType(string dataToSign, int hashSize, Blake2Wrapper wrapper)
        {
            if (hashSize == 256)
            {
                return await wrapper.Blake2256Async(dataToSign);
            }
            return await wrapper.Blake2512Async(dataToSign);
        }

        #endregion

        #region HMAC
        public async Task<IActionResult> HMACSign(HMACSignRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.Key) || string.IsNullOrEmpty(body.Message))
                {
                    result = new BadRequestObjectResult(new { error = "Need to provide a message and a key to sign with HMAC" });
                }
                else
                {
                    HmacWrapper hmac = new HmacWrapper();
                    IntPtr signaturePtr = await hmac.HmacSignAsync(body.Key, body.Message);
                    string signature = Marshal.PtrToStringAnsi(signaturePtr);
                    HmacWrapper.free_cstring(signaturePtr);
                    result = new OkObjectResult(new HMACSignResponse() { Signature = signature });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> HMACVerify(HMACVerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.Key) || string.IsNullOrEmpty(body.Message) || string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { error = "Need to provide a message, key, and signature to verfiy with HMAC" });
                }
                else
                {
                    HmacWrapper hmac = new HmacWrapper();
                    bool isValid = await hmac.HmacVerifyAsync(body.Key, body.Message, body.Signature);
                    result = new OkObjectResult(new HMACVerifyResponse() { IsValid = isValid });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region SHA512Ed25519Dalek
        public async Task<IActionResult> SHA512ED25519DalekSign(SHA512ED25519DalekSignRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.DataToSign))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to sign" });
                }
                else
                {
                    RustSHAWrapper shaWrapper = new RustSHAWrapper();
                    ED25519Wrapper ed25519Wrapper = new ED25519Wrapper();
                    List<Task> tasks = new List<Task>();
                    Task<IntPtr> hashedDataPtr = shaWrapper.SHA512HashStringAsync(body.DataToSign);
                    tasks.Add(hashedDataPtr);
                    Task<IntPtr> ed25519KeyPair = ed25519Wrapper.GetKeyPairAsync();
                    tasks.Add(ed25519KeyPair);
                    await Task.WhenAll(tasks);
                    string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr.Result);
                    string keyPair = Marshal.PtrToStringAnsi(ed25519KeyPair.Result);
                    Ed25519SignatureResult signatureResult = await ed25519Wrapper.SignAsync(keyPair, hashedData);
                    result = new OkObjectResult(new SHA512ED25519DalekSignResponse()
                    {
                        Signature = Marshal.PtrToStringAnsi(signatureResult.Signature),
                        PublicKey = Marshal.PtrToStringAnsi(signatureResult.Public_Key)
                    });
                    RustSHAWrapper.free_cstring(hashedDataPtr.Result);
                    ED25519Wrapper.free_cstring(ed25519KeyPair.Result);
                    ED25519Wrapper.free_cstring(signatureResult.Public_Key);
                    ED25519Wrapper.free_cstring(signatureResult.Signature);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> SHA512ED25519DalekVerify(SHA512ED25519DalekVerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a signature to verify with" });
                }
                else if (string.IsNullOrEmpty(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a public key to verify with" });
                }
                else if (string.IsNullOrEmpty(body.DataToVerify))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to verify with" });
                }
                else
                {
                    RustSHAWrapper shaWrapper = new RustSHAWrapper();
                    ED25519Wrapper ed25519Wrapper = new ED25519Wrapper();
                    IntPtr hashedDataToVerify = await shaWrapper.SHA512HashStringAsync(body.DataToVerify);
                    string dataToVerify = Marshal.PtrToStringAnsi(hashedDataToVerify);
                    bool isValid = await ed25519Wrapper.VerifyWithPublicAsync(body.PublicKey, body.Signature, dataToVerify);
                    result = new OkObjectResult(new SHA512ED25519DalekVerifyResponse()
                    {
                        IsValid = isValid
                    });
                    RustSHAWrapper.free_cstring(hashedDataToVerify);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion


        #region SHA512RSA
        public async Task<IActionResult> SHA512SignedRsa(SHA512SignedRSARequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.DataToHash))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide some data to hash" });
                }
                else if (body.KeySize != 1024 && body.KeySize != 2048 && body.KeySize != 4096)
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a valid key length for the RSA key generation" });
                }
                else
                {
                    List<Task> taskArray = new List<Task>();
                    RustSHAWrapper shaWrapper = new RustSHAWrapper();
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    Task<IntPtr> hashedDataPtr = shaWrapper.SHA512HashStringAsync(body.DataToHash);
                    taskArray.Add(hashedDataPtr);
                    Task<RustRsaKeyPair> keyPair = rsaWrapper.GetKeyPairAsync(body.KeySize);
                    taskArray.Add(keyPair);
                    await Task.WhenAll(taskArray);
                    string privateKey = Marshal.PtrToStringAnsi(keyPair.Result.priv_key);
                    string publicKey = Marshal.PtrToStringAnsi(keyPair.Result.pub_key);
                    RSAValidator rsaValidator = new RSAValidator();
                    if (!rsaValidator.IsPublicKeyPEMValid(publicKey) || !rsaValidator.IsPrivateKeyPEMValid(privateKey))
                    {
                        result = new BadRequestObjectResult(new { error = "We experienced a problem generating your SHA512 Digital Signature" });
                    }
                    else
                    {
                        string hashedData = Marshal.PtrToStringAnsi(hashedDataPtr.Result);
                        IntPtr signatureResultPtr = await rsaWrapper.RsaSignWithKeyAsync(privateKey, hashedData);
                        string signature = Marshal.PtrToStringAnsi(signatureResultPtr);
                        result = new OkObjectResult(new SHA512SignedRSAResponse()
                        {
                            Signature = signature,
                            PrivateKey = privateKey,
                            PublicKey = publicKey
                        });
                        RustRSAWrapper.free_cstring(signatureResultPtr);
                    }
                    RustRSAWrapper.free_cstring(hashedDataPtr.Result);
                    RustRSAWrapper.free_cstring(keyPair.Result.priv_key);
                    RustRSAWrapper.free_cstring(keyPair.Result.pub_key);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "We experienced a problem generating your SHA512 Digital Signature" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> SHA512SignedRsaVerify(SHA512SignedRSAVerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PublicKey) || !new RSAValidator().IsPublicKeyPEMValid(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide an RSA key to verify" });
                }
                else if (string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a Signature to verify" });
                }
                else if (string.IsNullOrEmpty(body.OriginalData))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide the originl data to verify" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    RustSHAWrapper shaWrapper = new RustSHAWrapper();
                    IntPtr originalHashPtr = await shaWrapper.SHA512HashStringAsync(body.OriginalData);
                    string originalHash = Marshal.PtrToStringAnsi(originalHashPtr);
                    bool isValid = await rsaWrapper.RsaVerifyAsync(body.PublicKey, originalHash, body.Signature);
                    result = new OkObjectResult(new SHA512SignedRSAVerifyResponse() { IsValid = isValid });
                    RustSHAWrapper.free_cstring(originalHashPtr);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}