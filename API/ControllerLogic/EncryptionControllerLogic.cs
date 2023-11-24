using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption;
using Models.Encryption.AESRSAHybrid;
using System.Reflection;
using System.Runtime.InteropServices;
using static Encryption.AESWrapper;
using static Encryption.RustRSAWrapper;

namespace API.ControllersLogic
{
    public class EncryptionControllerLogic : IEncryptionControllerLogic
    {
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        private readonly IEASExceptionRepository _exceptionRepository;
        public EncryptionControllerLogic(
            BenchmarkMethodCache benchmarkMethodCache,
            IEASExceptionRepository exceptionRepository)
        {
            this._benchmarkMethodCache = benchmarkMethodCache;
            this._exceptionRepository = exceptionRepository;
        }
        #region DecryptAES
        public async Task<IActionResult> DecryptAES(DecryptAESRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.AesType != 128 && body.AesType != 256)
                {
                    result = new BadRequestObjectResult(new { message = "Did not select a supported AES type" });
                }
                else
                {
                    if (!string.IsNullOrEmpty(body.DataToDecrypt) && !string.IsNullOrEmpty(body.Key) && !string.IsNullOrEmpty(body.NonceKey))
                    {
                        AESWrapper aes = new AESWrapper(new ZSTDWrapper());
                        IntPtr decryptedPtr = await this.GetAesDecryptType(body.NonceKey, body.Key, body.DataToDecrypt, body.AesType, aes);
                        string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
                        AESWrapper.free_cstring(decryptedPtr);
                        result = new OkObjectResult(new DecryptAESResponse() { Decrypted = decrypted });
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        private async Task<IntPtr> GetAesDecryptType(string nonceKey, string key, string dataToDecrypt, int aesType, AESWrapper aes)
        {
            if (aesType == 128)
                return await aes.DecryptAES128WithKeyAsync(nonceKey, key, dataToDecrypt);

            return await aes.DecryptPerformantAsync(nonceKey, key, dataToDecrypt);
        }

        #endregion

        #region EncryptAES
        public async Task<IActionResult> EncryptAES(EncryptAESRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.AesType != 128 && body.AesType != 256)
                {
                    result = new BadRequestObjectResult(new { error = "AES encryption type not supported" });
                }
                else
                {
                    if (!string.IsNullOrEmpty(body.DataToEncrypt) && !string.IsNullOrEmpty(body.NonceKey))
                    {
                        AESWrapper aes = new AESWrapper(new ZSTDWrapper());
                        AesEncrypt encrypted = await this.GetAesEncryptType(body.NonceKey, body.DataToEncrypt, body.AesType, aes);
                        string key = Marshal.PtrToStringAnsi(encrypted.key);
                        string cipherText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
                        AESWrapper.free_cstring(encrypted.key);
                        AESWrapper.free_cstring(encrypted.ciphertext);
                        result = new OkObjectResult(new EncryptAESResponse { Nonce = body.NonceKey, Key = key, Encrypted = cipherText });
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        private async Task<AesEncrypt> GetAesEncryptType(string nonceKey, string dataToEncrypt, int aesType, AESWrapper aes)
        {
            if (aesType == 128)
                return await aes.Aes128EncryptAsync(nonceKey, dataToEncrypt);

            // Default value of AES 256
            return await aes.EncryptPerformantAsync(nonceKey, dataToEncrypt);
        }

        #endregion

        #region EncryptSHA
        public async Task<IActionResult> EncryptSHA512(EncryptSHARequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.DataToEncrypt))
                {
                    RustSHAWrapper sha = new RustSHAWrapper();
                    IntPtr hashToReturnPtr = await sha.SHA512HashStringAsync(body.DataToEncrypt);
                    string hashToReturn = Marshal.PtrToStringAnsi(hashToReturnPtr);
                    RustSHAWrapper.free_cstring(hashToReturnPtr);
                    result = new OkObjectResult(new HashSHAResponse() { Hash = hashToReturn });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> EncryptSHA256(EncryptSHARequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.DataToEncrypt))
                {
                    RustSHAWrapper sha = new RustSHAWrapper();
                    IntPtr hashToReturnPtr = await sha.SHA256HashStringAsync(body.DataToEncrypt);
                    string hashToReturn = Marshal.PtrToStringAnsi(hashToReturnPtr);
                    RustSHAWrapper.free_cstring(hashToReturnPtr);
                    result = new OkObjectResult(new HashSHAResponse() { Hash = hashToReturn });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion

        #region HashMD5
        public async Task<IActionResult> HashMD5(MD5Request body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.DataToHash))
                {
                    MD5Wrapper md5 = new MD5Wrapper();
                    IntPtr hashPtr = await md5.HashAsync(body.DataToHash);
                    string hash = Marshal.PtrToStringAnsi(hashPtr);
                    MD5Wrapper.free_cstring(hashPtr);
                    result = new OkObjectResult(new MD5HashResponse() { Hash = hash });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "You need to specific data to hash" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region VerifyMD5
        public async Task<IActionResult> VerifyMD5(MD5VerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.HashToVerify) && !string.IsNullOrEmpty((body.ToHash)))
                {
                    MD5Wrapper md5 = new MD5Wrapper();
                    bool isValid = await md5.VerifyAsync(body.HashToVerify, body.ToHash);
                    result = new OkObjectResult(new MD5VerifyResponse() { IsValid = isValid });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "You need a MD5 hash and data to hash to verify a MD5 hash" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region EncryptAESRSAHybrid
        public async Task<IActionResult> EncryptAESRSAHybrid(AESRSAHybridEncryptRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.AesType != 128 && body.AesType != 256)
                {
                    result = new BadRequestObjectResult(new { error = "AES encryption type not supported" });
                }
                else if (string.IsNullOrEmpty(body.Nonce))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a nonce to perform hybrid aes and hybrid encryption" });
                }
                else if (string.IsNullOrEmpty(body.DataToEncrypt))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to encrypt" });
                }
                else if (body.KeySize != 1024 && body.KeySize != 2048 && body.KeySize != 4096)
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a RSA key length to encrypt" });
                }
                else
                {
                    AESWrapper aesWrapper = new AESWrapper(new ZSTDWrapper());
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    List<Task> tasks = new List<Task>();
                    Task<RustRsaKeyPair> keyPair = rsaWrapper.GetKeyPairAsync(body.KeySize);
                    tasks.Add(keyPair);
                    Task<AesEncrypt> aesEncryptResult = this.GetAesEncryptType(body.Nonce, body.DataToEncrypt, body.AesType, aesWrapper);
                    tasks.Add(aesEncryptResult);
                    await Task.WhenAll(tasks);
                    string encryptedData = Marshal.PtrToStringAnsi(aesEncryptResult.Result.ciphertext);
                    string aesKey = Marshal.PtrToStringAnsi(aesEncryptResult.Result.key);
                    string publicKey = Marshal.PtrToStringAnsi(keyPair.Result.pub_key);
                    string privateKey = Marshal.PtrToStringAnsi(keyPair.Result.priv_key);
                    IntPtr encryptedAesKeyPtr = await rsaWrapper.RsaEncryptAsync(publicKey, aesKey);
                    string encryptedAesKey = Marshal.PtrToStringAnsi(encryptedAesKeyPtr);
                    result = new OkObjectResult(new AESRSAHybridEncryptResponse()
                    {
                        EncryptedAesKey = encryptedAesKey,
                        EncryptedData = encryptedData,
                        PublicKey = publicKey,
                        PrivateKey = privateKey
                    });
                    RustRSAWrapper.free_cstring(keyPair.Result.pub_key);
                    RustRSAWrapper.free_cstring(keyPair.Result.priv_key);
                    RustRSAWrapper.free_cstring(encryptedAesKeyPtr);
                    AESWrapper.free_cstring(aesEncryptResult.Result.key);
                    AESWrapper.free_cstring(aesEncryptResult.Result.ciphertext);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region DecryptAESRSAHybrid

        public async Task<IActionResult> DecryptAESRSAHybrid(AESRSAHybridDecryptRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (body.AesType != 128 && body.AesType != 256)
                {
                    result = new BadRequestObjectResult(new { error = "AES decryption type not supported" });
                }
                else if (string.IsNullOrEmpty(body.PrivateRsaKey))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide and RSA Private Key to Decrypt" });
                }
                else if (string.IsNullOrEmpty(body.EncryptedAesKey))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a AES key to decrypt" });
                }
                else if (string.IsNullOrEmpty(body.Nonce))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a nonce to decrpyt" });
                }
                else if (string.IsNullOrEmpty(body.EncryptedData))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a data to decrypt" });
                }
                else
                {
                    AESWrapper aesWrapper = new AESWrapper(new ZSTDWrapper());
                    RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                    IntPtr decryptedAesKeyPtr = await rsaWrapper.RsaDecryptAsync(body.PrivateRsaKey, body.EncryptedAesKey);
                    string decryptedAesKey = Marshal.PtrToStringAnsi(decryptedAesKeyPtr);
                    IntPtr decryptedDataPtr = await this.GetAesDecryptType(body.Nonce, decryptedAesKey, body.EncryptedData, body.AesType, aesWrapper);
                    string decryptedData = Marshal.PtrToStringAnsi(decryptedDataPtr);
                    result = new OkObjectResult(new AESRSAHybridDecryptResponse()
                    {
                        DecryptedData = decryptedData
                    });
                    AESWrapper.free_cstring(decryptedDataPtr);
                    RustRSAWrapper.free_cstring(decryptedAesKeyPtr);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region Blake2
        public async Task<IActionResult> Blake2Hash(Blake2Request body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {

                if (string.IsNullOrEmpty(body.DataToHash))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to hash with Blake2" });
                }
                else if (body.HashSize != 256 && body.HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "Blake2 hash size not supported" });
                }
                else
                {
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    IntPtr hashPtr = await this.GetBlake2HashType(body.HashSize, body.DataToHash, blake2Wrapper);
                    string hash = Marshal.PtrToStringAnsi(hashPtr);
                    result = new OkObjectResult(new Blake2Response()
                    {
                        HashedData = hash
                    });
                    Blake2Wrapper.free_cstring(hashPtr);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        private async Task<IntPtr> GetBlake2HashType(int hashSize, string dataToHash, Blake2Wrapper blake2Wrapper)
        {
            if (hashSize == 256)
                return await blake2Wrapper.Blake2256Async(dataToHash);

            return await blake2Wrapper.Blake2512Async(dataToHash);
        }

        public async Task<IActionResult> Blake2Verify([FromBody] Blake2VerifyRequest body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {

                if (string.IsNullOrEmpty(body.DataToVerify))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide data to verify with Blake2" });
                }
                else if (body.HashSize != 256 && body.HashSize != 512)
                {
                    result = new BadRequestObjectResult(new { error = "Blake2 hash size not supported" });
                }
                else if (string.IsNullOrEmpty(body.Hash))
                {
                    result = new BadRequestObjectResult(new { error = "You must provide a hash to verify with Blake2" });
                }
                else
                {
                    Blake2Wrapper blake2Wrapper = new Blake2Wrapper();
                    bool isValid = await this.GetBlake2VerifyType(body.HashSize, body.DataToVerify, body.Hash, blake2Wrapper);
                    result = new OkObjectResult(new Blake2VerifyResponse() { IsValid = isValid });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestResult();
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        private async Task<bool> GetBlake2VerifyType(int hashSize, string dataToVerify, string hash, Blake2Wrapper blake2Wrapper)
        {
            if (hashSize == 256)
                return await blake2Wrapper.Blake2256VerifyAsync(dataToVerify, hash);

            return await blake2Wrapper.Blake2512VerifyAsync(dataToVerify, hash);
        }
        #endregion
    }
}