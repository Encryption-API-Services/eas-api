using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Encryption.PasswordHash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Encryption;
using Models.UserAuthentication;
using MongoDB.Bson;
using System.Reflection;
using System.Runtime.InteropServices;
using Validation.UserRegistration;
using User = DataLayer.Mongo.Entities.User;

namespace API.ControllersLogic
{
    public class PasswordControllerLogic : IPasswordControllerLogic
    {
        private readonly IHashedPasswordRepository _hashedPasswordRepository;
        private readonly IUserRepository _userRepository;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        public PasswordControllerLogic(
            IHashedPasswordRepository hashedPasswordRepository,
            IUserRepository userRepository,
            IForgotPasswordRepository forgotPasswordRepository,
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache
            )
        {
            this._hashedPasswordRepository = hashedPasswordRepository;
            this._userRepository = userRepository;
            this._forgotPasswordRepository = forgotPasswordRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
        }

        #region Argon2Hash
        public async Task<IActionResult> Argon2Hash(Argon2HashRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.passwordToHash))
                {
                    Argon2Wrappper argon2 = new Argon2Wrappper();
                    IntPtr hashedPasswordPtr = argon2.HashPassword(body.passwordToHash);
                    string hashedPassowrd = Marshal.PtrToStringUTF8(hashedPasswordPtr);
                    Argon2Wrappper.free_cstring(hashedPasswordPtr);
                    await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                    result = new OkObjectResult(new Argon2HashResponse() { HashedPassword = hashedPassowrd });
                }
                else
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a password to hash with argon2" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> Argon2HashBatch(Argon2HashBatchRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (body.Passwords.Count <= 0)
                {
                    result = new BadRequestObjectResult(new { error = "You must provide passwords to hash" });
                }
                else
                {
                    Argon2Wrappper argon2 = new Argon2Wrappper();
                    List<Task<IntPtr>> encryptedPtrs = new List<Task<IntPtr>>();
                    for (int i = 0; i < body.Passwords.Count; i++)
                    {
                        encryptedPtrs.Add(argon2.HashPasswordAsync(body.Passwords[i]));
                    }
                    await Task.WhenAll(encryptedPtrs);
                    List<string> hashedPasswords = new List<string>();
                    for (int j = 0; j < encryptedPtrs.Count; j++)
                    {
                        hashedPasswords.Add(Marshal.PtrToStringAnsi(encryptedPtrs[j].Result));
                        Argon2Wrappper.free_cstring(encryptedPtrs[j].Result);
                    }
                    result = new OkObjectResult(new Argon2HashBatchResponse() { HashedPasswords = hashedPasswords });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion
        #region Argon2Verify
        public async Task<IActionResult> Argon2Verify(Argon2VerifyRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.hashedPassword) && !string.IsNullOrEmpty(body.password))
                {
                    Argon2Wrappper argon2 = new Argon2Wrappper();
                    bool isValid = await argon2.VerifyPasswordAsync(body.hashedPassword, body.password);
                    await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                    result = new OkObjectResult(new { IsValid = isValid });
                }
                else
                {
                    result = new BadRequestObjectResult(new { message = "You must provide a hashed password and a password to verify with argon2" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> BcryptEncryptBatch(BCryptEncryptBatchRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (body.Passwords.Count <= 0)
                {
                    result = new BadRequestObjectResult(new { message = "You must provide passwords to hash." });
                }
                else
                {
                    BcryptWrapper bcryptWrapper = new BcryptWrapper();
                    List<Task<IntPtr>> encryptTasks = new List<Task<IntPtr>>();
                    for (int i = 0; i < body.Passwords.Count; i++)
                    {
                        encryptTasks.Add(bcryptWrapper.HashPasswordAsync(body.Passwords[i]));
                    }
                    await Task.WhenAll(encryptTasks);
                    List<string> hashedPasswords = new List<string>();
                    for (int j = 0; j < encryptTasks.Count; j++)
                    {
                        hashedPasswords.Add(Marshal.PtrToStringAnsi(encryptTasks[j].Result));
                        BcryptWrapper.free_cstring(encryptTasks[j].Result);
                    }
                    result = new OkObjectResult(new BCryptEncryptBatchResponse { HashedPasswords = hashedPasswords });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region BcryptEncryprt
        public async Task<IActionResult> BcryptEncryptPassword([FromBody] BCryptEncryptModel body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.Password))
                {
                    BcryptWrapper bcrypt = new BcryptWrapper();
                    IntPtr hashedPasswordPtr = await bcrypt.HashPasswordAsync(body.Password);
                    string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
                    BcryptWrapper.free_cstring(hashedPasswordPtr);
                    await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                    result = new OkObjectResult(new BCryptEncryptResponseModel() { HashedPassword = hashedPassword });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region BcryptVerify
        public async Task<IActionResult> BcryptVerifyPassword([FromBody] BcryptVerifyModel body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.Password) && !string.IsNullOrEmpty(body.HashedPassword))
                {
                    BcryptWrapper wrapper = new BcryptWrapper();
                    bool valid = await wrapper.VerifyAsync(body.HashedPassword, body.Password);
                    await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                    result = new OkObjectResult(new BCryptVerifyResponseModel() { IsValid = valid });
                }
                else
                {
                    result = new BadRequestObjectResult(new { });
                }

            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ForgotPassword
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                RegisterUserValidation validator = new RegisterUserValidation();
                if (validator.IsEmailValid(body.Email))
                {
                    User databaseUser = await this._userRepository.GetUserByEmail(body.Email);
                    ForgotPassword forgotPassword = new ForgotPassword()
                    {
                        Token = Guid.NewGuid().ToString(),
                        HasBeenReset = false
                    };
                    await this._userRepository.UpdateForgotPassword(databaseUser.Id, forgotPassword);
                    result = new OkObjectResult(new { message = "You should be expecting an email to reset your password soon." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ResetPassword
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                User databaseUser = await this._userRepository.GetUserById(body.Id);
                RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                string signedToken = Base64UrlEncoder.Decode(body.Token);
                bool isValid = await rsaWrapper.RsaVerifyAsync(databaseUser.ForgotPassword.PublicKey, databaseUser.ForgotPassword.Token, signedToken);
                if (isValid)
                {
                    Argon2Wrappper wrapper = new Argon2Wrappper();
                    IntPtr hashedPasswordPtr = await wrapper.HashPasswordAsync(body.Password);
                    string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
                    List<string> lastFivePasswords = await this._forgotPasswordRepository.GetLastFivePassword(body.Id);
                    foreach (string password in lastFivePasswords)
                    {
                        if (await wrapper.VerifyPasswordAsync(password, body.Password))
                        {
                            result = new BadRequestObjectResult(new { error = "You need to enter a password that has not been used the last 5 times" });
                            return result;
                        }
                    }
                    await this._userRepository.UpdatePassword(databaseUser.Id, hashedPassword);
                    await this._forgotPasswordRepository.InsertForgotPasswordAttempt(databaseUser.Id, hashedPassword);
                    result = new OkObjectResult(new { message = "You have successfully changed your password." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> SCryptEncryptBatch(SCryptEncryptBatchRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (body.Passwords.Count <= 0)
                {
                    result = new BadRequestObjectResult(new { error = "You must provide passwords to hash" });
                }
                else
                {
                    SCryptWrapper scrypt = new SCryptWrapper();
                    List<Task<IntPtr>> hashedPasswodsPtr = new List<Task<IntPtr>>();
                    for (int i = 0; i < body.Passwords.Count; i++)
                    {
                        hashedPasswodsPtr.Add(scrypt.HashPasswordAsync(body.Passwords[i]));
                    }
                    await Task.WhenAll(hashedPasswodsPtr);
                    List<string> hashedPasswords = new List<string>();
                    for (int j = 0; j < hashedPasswodsPtr.Count; j++)
                    {
                        hashedPasswords.Add(Marshal.PtrToStringAnsi(hashedPasswodsPtr[j].Result));
                        SCryptWrapper.free_cstring(hashedPasswodsPtr[j].Result);
                    }
                    result = new OkObjectResult(new SCryptEncryptBatchResponse() { HashedPasswords = hashedPasswords });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ScryptHash
        public async Task<IActionResult> ScryptHashPassword(ScryptHashRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                SCryptWrapper scrypt = new SCryptWrapper();
                IntPtr hashedPasswordPtr = await scrypt.HashPasswordAsync(body.passwordToHash);
                string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
                SCryptWrapper.free_cstring(hashedPasswordPtr);
                await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                result = new OkObjectResult(new SCryptHashResponse { HashedPassword = hashedPassword });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ScryptVerify

        public async Task<IActionResult> ScryptVerifyPassword(SCryptVerifyRequest body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.hashedPassword) && !string.IsNullOrEmpty(body.password))
                {
                    SCryptWrapper scrypt = new SCryptWrapper();
                    bool isValid = await scrypt.VerifyPasswordAsync(body.password, body.hashedPassword);
                    await this.InsertHashedPasswordMethodRecord(context, MethodBase.GetCurrentMethod().Name);
                    result = new OkObjectResult(new { isValid = isValid });
                }
                else
                {
                    result = new BadRequestObjectResult(new { message = "You need to provide a hashed password and password to verify." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region Helpers
        private async Task InsertHashedPasswordMethodRecord(HttpContext context, string methodName)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = new JWT().GetUserIdFromToken(token);
            HashedPassword newPassword = new HashedPassword()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                HashMethod = methodName,
                CreateDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            await this._hashedPasswordRepository.InsertOneHasedPassword(newPassword);
        }
        #endregion
    }
}
