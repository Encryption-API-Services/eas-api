using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.PasswordHashers;
using CasDotnetSdk.Signatures;
using CASHelpers;
using Common;
using Common.ThirdPartyAPIs;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using DataLayer.RabbitMQ.QueueMessages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.UserAuthentication;
using MongoDB.Driver;
using OtpNet;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace API.ControllersLogic
{
    public class UserLoginControllerLogic : IUserLoginControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IFailedLoginAttemptRepository _failedLoginAttemptRepository;
        private readonly IHotpCodesRepository _hotpCodesRepository;
        private readonly ISuccessfulLoginRepository _successfulLoginRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly LockedOutUserQueuePublish _lockedOutUserQueue;

        public UserLoginControllerLogic(
            IUserRepository userRepository,
            IFailedLoginAttemptRepository failedLoginAttemptRepository,
            IHotpCodesRepository hotpCodesRepository,
            ISuccessfulLoginRepository successfulLoginRepository,
            ICASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache,
            LockedOutUserQueuePublish lockedOutUserQueue
            )
        {
            this._userRepository = userRepository;
            this._failedLoginAttemptRepository = failedLoginAttemptRepository;
            this._hotpCodesRepository = hotpCodesRepository;
            this._successfulLoginRepository = successfulLoginRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchmarkMethodCache;
            this._lockedOutUserQueue = lockedOutUserQueue;
        }

        #region GetApiKey
        public async Task<IActionResult> GetApiKey(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string token = context.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
                // get current token
                if (!string.IsNullOrEmpty(token))
                {
                    JWT jwtWrapper = new JWT();
                    string userId = jwtWrapper.GetUserIdFromToken(token);
                    string apiKey = await this._userRepository.GetApiKeyById(userId);
                    result = new OkObjectResult(new { apiKey = apiKey });
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

        #region GetSuccessfulLogins
        public async Task<IActionResult> GetSuccessfulLogins(HttpContext context, int pageSkip, int pageSize)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                IFindFluent<SuccessfulLogin, SuccessfulLogin> successfulLogins = this._successfulLoginRepository.GetAllSuccessfulLoginWithinTimeFrame(userId, DateTime.UtcNow.AddMonths(-1));
                Task<long> total = successfulLogins.CountDocumentsAsync();
                Task<List<SuccessfulLogin>> logins = successfulLogins.Skip(pageSkip * pageSize).Limit(pageSize).ToListAsync();
                await Task.WhenAll(total, logins);
                result = new OkObjectResult(new { Total = total.Result, Logins = logins.Result });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end getting the recent login activity." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region LoginUser
        public async Task<IActionResult> LoginUser(LoginUser body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                User activeUser = await this._userRepository.GetUserByEmail(body.Email);
                if (activeUser == null)
                {
                    result = new BadRequestObjectResult(new { error = "This user account does not exist in our system." });
                }
                else if (activeUser != null && activeUser.IsActive == false)
                {
                    result = new BadRequestObjectResult(new { error = "Check your email for an account activation email." });
                }
                else if (activeUser != null && activeUser.LockedOut.IsLockedOut == false && activeUser.IsActive == true)
                {
                    Argon2Wrapper argon2 = new Argon2Wrapper();
                    if (argon2.VerifyPassword(activeUser.Password, body.Password))
                    {
                        ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                        string token = new JWT().GenerateECCToken(activeUser.Id, activeUser.IsAdmin, ecdsa, 1);
                        if (activeUser.Phone2FA != null && activeUser.Phone2FA.IsEnabled)
                        {
                            byte[] secretKey = KeyGeneration.GenerateRandomKey(OtpHashMode.Sha512);
                            long counter = await this._hotpCodesRepository.GetHighestCounter() + 1;
                            Hotp hotpGenerator = new Hotp(secretKey, OtpHashMode.Sha512, 8);
                            HotpCode code = new HotpCode()
                            {
                                UserId = activeUser.Id,
                                Counter = counter,
                                Hotp = hotpGenerator.ComputeHOTP(counter),
                                HasBeenSent = false,
                                HasBeenVerified = false
                            };
                            await this._hotpCodesRepository.InsertHotpCode(code);
                            result = new OkObjectResult(new { message = "You need to verify the code sent to your phone.", TwoFactorAuth = true });
                        }
                        else
                        {
                            IpInfoHelper ipInfoHelper = new IpInfoHelper();
                            IpInfoResponse ipInfo = await ipInfoHelper.GetIpInfo(httpContext.Items[Constants.HttpItems.IP].ToString());
                            SuccessfulLogin login = new SuccessfulLogin()
                            {
                                UserId = activeUser.Id,
                                Ip = httpContext.Items[Constants.HttpItems.IP].ToString(),
                                UserAgent = body.UserAgent,
                                City = ipInfo.City,
                                Country = ipInfo.Country,
                                TimeZone = ipInfo.TimeZone,
                                CreateTime = DateTime.UtcNow
                            };
                            await this._successfulLoginRepository.InsertSuccessfulLogin(login);
                            result = new OkObjectResult(new { message = "You have successfully signed in.", token = token, TwoFactorAuth = false });
                        }
                    }
                    else
                    {
                        FailedLoginAttempt attempt = new FailedLoginAttempt()
                        {
                            Password = body.Password,
                            CreateDate = DateTime.UtcNow,
                            LastModifed = DateTime.UtcNow,
                            UserAccount = activeUser.Id
                        };
                        await this._failedLoginAttemptRepository.InsertFailedLoginAttempt(attempt);
                        List<FailedLoginAttempt> lastTwelveHourAttempts = await this._failedLoginAttemptRepository.GetFailedLoginAttemptsLastTweleveHours(activeUser.Id);
                        if (lastTwelveHourAttempts.Count >= 5)
                        {
                            await this._userRepository.LockoutUser(activeUser.Id);
                            LockedOutUserQueueMessage newMessage = new LockedOutUserQueueMessage()
                            {
                                UserEmail = activeUser.Email,
                                UserId = activeUser.Id
                            };
                            this._lockedOutUserQueue.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newMessage)));
                            result = new BadRequestObjectResult(new { error = "You entered an invalid password, your account has been locked due to many attempts." });
                        }
                        else
                        {
                            result = new BadRequestObjectResult(new { error = "You entered an invalid password" });
                        }
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "This user account has been locked out due to many failed login attempts." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end. Please try again." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region UnlockUser
        public async Task<IActionResult> UnlockUser(UnlockUser body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.Id) && !string.IsNullOrEmpty(body.Token))
                {
                    User databaseUser = await this._userRepository.GetUserById(body.Id);
                    byte[] signedToken = Base64UrlEncoder.DecodeBytes(body.Token);
                    ED25519Wrapper ed25519 = new ED25519Wrapper();
                    bool isValid = ed25519.VerifyWithPublicKeyBytes(Convert.FromBase64String(databaseUser.LockedOut.PublicKey), signedToken, Convert.FromBase64String(databaseUser.LockedOut.Token));
                    if (isValid)
                    {
                        await this._userRepository.UnlockUser(body.Id);
                        result = new OkResult();
                    }
                    else
                    {
                        result = new BadRequestResult();
                    }
                }
                else
                {
                    result = new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> ValidateHotpCode([FromBody] ValidateHotpCode body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                // get hotp code by userId and HotpCode
                HotpCode databaseCode = await this._hotpCodesRepository.GetHotpCodeByIdAndCode(body.UserId);
                if (databaseCode != null)
                {
                    Hotp hotp = new Hotp(databaseCode.SecretKey, OtpHashMode.Sha512, 8);
                    bool isValid = hotp.VerifyHotp(body.HotpCode, databaseCode.Counter);
                    if (isValid)
                    {
                        await this._hotpCodesRepository.UpdateHotpToVerified(databaseCode.Id);
                        User activeUser = await this._userRepository.GetUserById(body.UserId);
                        IpInfoHelper ipInfoHelper = new IpInfoHelper();
                        IpInfoResponse ipInfo = await ipInfoHelper.GetIpInfo(context.Items[Constants.HttpItems.IP].ToString());
                        SuccessfulLogin login = new SuccessfulLogin()
                        {
                            UserId = activeUser.Id,
                            Ip = context.Items[Constants.HttpItems.IP].ToString(),
                            UserAgent = body.UserAgent,
                            City = ipInfo.City,
                            Country = ipInfo.Country,
                            TimeZone = ipInfo.TimeZone,
                            CreateTime = DateTime.UtcNow
                        };
                        await this._successfulLoginRepository.InsertSuccessfulLogin(login);
                        ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                        string token = new JWT().GenerateECCToken(activeUser.Id, activeUser.IsAdmin, ecdsa, 1);
                        result = new OkObjectResult(new { message = "You have successfully verified your authentication code.", token = token });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = "The authentication code that you entered was invalid" });
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "The authentication code that you entered was invalid" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion

        #region WasLoginMe
        public async Task<IActionResult> WasLoginMe(WasLoginMe body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                await this._successfulLoginRepository.UpdateSuccessfulLoginWasMe(body.LoginId, body.WasMe);
                result = new OkResult();
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
    }
}
