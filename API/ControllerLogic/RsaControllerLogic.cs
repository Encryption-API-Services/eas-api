using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using System.Reflection;
using System.Runtime.InteropServices;
using Validation.Keys;
using static Encryption.RustRSAWrapper;

namespace API.ControllerLogic
{
    public class RsaControllerLogic : IRsaControllerLogic
    {
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly IRsaEncryptionRepository _rsaEncryptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        public RsaControllerLogic(
            IEASExceptionRepository exceptionRepository,
            IRsaEncryptionRepository rsaEncryptionRepository,
            BenchmarkMethodCache benchMarkMethodCache
            )
        {
            this._exceptionRepository = exceptionRepository;
            this._rsaEncryptionRepository = rsaEncryptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
        }

        #region Decrypt
        public async Task<IActionResult> DecryptWithStoredPrivate(HttpContext context, RsaDecryptWithStoredPrivateRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PublicKey) || !new RSAValidator().IsPublicKeyPEMValid(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide a public key and data to decrypt" });
                }
                else if (string.IsNullOrEmpty(body.DataToDecrypt))
                {
                    result = new BadRequestObjectResult(new { message = "You need data to decrypt." });
                }
                else
                {
                    string userId = context.Items["UserID"].ToString();
                    RsaEncryption rsaEncryption = await this._rsaEncryptionRepository.GetEncryptionByIdAndPublicKey(userId, body.PublicKey);
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    IntPtr decryptedDataPtr = await rsaWrapper.RsaDecryptAsync(rsaEncryption.PrivateKey, body.DataToDecrypt);
                    string decryptedData = Marshal.PtrToStringAnsi(decryptedDataPtr);
                    RustRSAWrapper.free_cstring(decryptedDataPtr);
                    result = new OkObjectResult(new RsaDecryptWithStoredPrivateResponse() { DecryptedData = decryptedData });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = " Something went wrong on our end while decrypting your data" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region EncryptWithoutPublic
        public async Task<IActionResult> EncryptWithoutPublic(HttpContext context, RsaEncryptWithoutPublicRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (body.keySize != 1024 && body.keySize != 2048 && body.keySize != 4096)
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a valid key size to encrypt your data" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    RustRsaKeyPair keyPair = await rsaWrapper.GetKeyPairAsync(body.keySize);
                    string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
                    string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
                    RSAValidator rsaValidator = new RSAValidator();
                    if (!rsaValidator.IsPublicKeyPEMValid(publicKey) || !rsaValidator.IsPrivateKeyPEMValid(privateKey))
                    {
                        result = new BadRequestObjectResult(new { message = "We experienced a problem Encrypting your data." });
                    }
                    else
                    {
                        IntPtr encryptedPtr = await rsaWrapper.RsaEncryptAsync(publicKey, body.dataToEncrypt);
                        string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
                        RsaEncryption rsaEncryption = new RsaEncryption()
                        {
                            UserId = context.Items["UserID"].ToString(),
                            PublicKey = publicKey,
                            PrivateKey = privateKey,
                            CreatedDate = DateTime.UtcNow
                        };
                        await this._rsaEncryptionRepository.InsertNewEncryption(rsaEncryption);
                        RustRSAWrapper.free_cstring(encryptedPtr);
                        result = new OkObjectResult(new RsaEncryptWithoutPublicResponse() { PublicKey = publicKey, EncryptedData = encrypted });
                    }
                    RustRSAWrapper.free_cstring(keyPair.pub_key);
                    RustRSAWrapper.free_cstring(keyPair.priv_key);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "External component has thrown an exception.")
                {
                    result = new BadRequestObjectResult(new { error = "The data you are trying to encrypt is too long for the RSA bit key length" });
                }
                else
                {
                    result = new BadRequestObjectResult(new { message = "We experienced a problem Encrypting your data." });
                }
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion

        #region EncryptWithPublic
        public async Task<IActionResult> EncryptWithPublic(HttpContext context, EncryptWithPublicRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PublicKey) && !new RSAValidator().IsPublicKeyPEMValid(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a public key we generated for you" });
                }
                else if (string.IsNullOrEmpty(body.DataToEncrypt))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide data to encrypt" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    IntPtr encryptedPtr = await rsaWrapper.RsaEncryptAsync(body.PublicKey, body.DataToEncrypt);
                    string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
                    RustRSAWrapper.free_cstring(encryptedPtr);
                    result = new OkObjectResult(new { EncryptedData = encrypted });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "External component has thrown an exception.")
                {
                    result = new BadRequestObjectResult(new { error = "The data you are trying to encrypt is too long for the RSA bit key length" });
                }
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region GetKeyPair
        public async Task<IActionResult> GetKeyPair(HttpContext context, int keySize)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (keySize != 1024 && keySize != 2048 && keySize != 4096)
                {
                    result = new BadRequestObjectResult(new { message = "You need to specify a valid key size to generate RSA keys" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    RustRsaKeyPair keyPair = await rsaWrapper.GetKeyPairAsync(keySize);
                    string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
                    string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
                    RSAValidator rsaValidator = new RSAValidator();
                    if (!rsaValidator.IsPublicKeyPEMValid(publicKey) || !rsaValidator.IsPrivateKeyPEMValid(privateKey))
                    {
                        result = new BadRequestObjectResult(new { message = "There was an error on our end generating a key pair for you" });
                    }
                    else
                    {
                        result = new OkObjectResult(new { PublicKey = publicKey, PrivateKey = privateKey });
                    }
                    RustRSAWrapper.free_cstring(keyPair.pub_key);
                    RustRSAWrapper.free_cstring(keyPair.priv_key);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "There was an error on our end generating a key pair for you" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion

        #region SignWithoutKey
        public async Task<IActionResult> SignWithoutKey(HttpContext context, RsaSignWithoutKeyRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.dataToSign))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide data to sign with RSA" });
                }
                else if (body.keySize != 1024 && body.keySize != 2048 && body.keySize != 4096)
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a valid RSA key bit size to sign your data" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    RsaSignResult rsaSignResult = await rsaWrapper.RsaSignAsync(body.dataToSign, body.keySize);
                    string publicKey = Marshal.PtrToStringAnsi(rsaSignResult.public_key);
                    RSAValidator rsaValidator = new RSAValidator();
                    if (!rsaValidator.IsPublicKeyPEMValid(publicKey))
                    {
                        result = new BadRequestObjectResult(new { message = "There was an error on our end signing your data for you" });
                    }
                    else
                    {
                        string signature = Marshal.PtrToStringAnsi(rsaSignResult.signature);
                        result = new OkObjectResult(new RsaSignWithoutKeyResponse() { PublicKey = publicKey, Signature = signature });
                    }
                    RustRSAWrapper.free_cstring(rsaSignResult.signature);
                    RustRSAWrapper.free_cstring(rsaSignResult.public_key);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "There was an error on our end signing your data for you" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region Verify
        public async Task<IActionResult> Verify(HttpContext context, RsaVerifyRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PublicKey) || !new RSAValidator().IsPublicKeyPEMValid(body.PublicKey))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a public key to verify" });
                }
                else if (string.IsNullOrEmpty(body.OriginalData))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide the original data to verify its signature" });
                }
                else if (string.IsNullOrEmpty(body.Signature))
                {
                    result = new BadRequestObjectResult(new { message = "You must provide the RSA signature we computed or you to verify with RSA" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    bool isValid = await rsaWrapper.RsaVerifyAsync(body.PublicKey, body.OriginalData, body.Signature);
                    result = new OkObjectResult(new RsaVerifyResponse() { IsValid = isValid });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "There was an error on our end verifying your data for you, did you provide the appropriate unaltered data?" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
            #endregion
        }

        #region VerifyWithKey
        public async Task<IActionResult> SignWithKey(HttpContext context, RsaSignWithKeyRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PrivateKey) || !new RSAValidator().IsPrivateKeyPEMValid(body.PrivateKey))
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide a private key to verify your data signature" });
                }
                else if (string.IsNullOrEmpty(body.DataToSign))
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide the data to sign" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    IntPtr signaturePtr = await rsaWrapper.RsaSignWithKeyAsync(body.PrivateKey, body.DataToSign);
                    string signature = Marshal.PtrToStringAnsi(signaturePtr);
                    RustRSAWrapper.free_cstring(signaturePtr);
                    result = new OkObjectResult(new RsaSignWithKeyResponse() { Signature = signature });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "There was an error on our end verifying your data for you, did you provide the appropriate unaltered data?" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> Decrypt(HttpContext context, RsaDecryptRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (string.IsNullOrEmpty(body.PrivateKey) || !new RSAValidator().IsPrivateKeyPEMValid(body.PrivateKey))
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide a private key to decrypt your data signature" });
                }
                else if (string.IsNullOrEmpty(body.DataToDecrypt))
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide the data to decrypt" });
                }
                else
                {
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    IntPtr decryptedDataPtr = await rsaWrapper.RsaDecryptAsync(body.PrivateKey, body.DataToDecrypt);
                    string decryptedData = Marshal.PtrToStringAnsi(decryptedDataPtr);
                    RustRSAWrapper.free_cstring(decryptedDataPtr);
                    result = new OkObjectResult(new RsaDecryptResponse() { DecryptedData = decryptedData });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { message = "There was an error on our end verifying your data for you, did you provide the appropriate unaltered data?" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}