using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.PasswordHashers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using DataLayer.RabbitMQ.QueueMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.UserAuthentication;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Validation.UserRegistration;
using User = DataLayer.Mongo.Entities.User;

namespace API.ControllersLogic
{
    public class PasswordControllerLogic : IPasswordControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly ForgotPasswordQueuePublish _forgotPasswordQueue;
        public PasswordControllerLogic(
            IUserRepository userRepository,
            IForgotPasswordRepository forgotPasswordRepository,
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache,
            ForgotPasswordQueuePublish forgotPasswordQueue
            )
        {
            this._userRepository = userRepository;
            this._forgotPasswordRepository = forgotPasswordRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
            this._forgotPasswordQueue = forgotPasswordQueue;
        }

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
                    ForgotPasswordQueueMessage newMessage = new ForgotPasswordQueueMessage()
                    {
                        UserEmail = databaseUser.Email,
                        UserId = databaseUser.Id
                    };
                    this._forgotPasswordQueue.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newMessage)));
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
                RSAWrapper rsaWrapper = new RSAWrapper();
                byte[] signedToken = Base64UrlEncoder.DecodeBytes(body.Token);
                bool isValid = rsaWrapper.RsaVerifyBytes(databaseUser.ForgotPassword.PublicKey, Convert.FromBase64String(databaseUser.ForgotPassword.Token), signedToken);
                if (isValid)
                {
                    List<string> lastFivePasswords = await this._forgotPasswordRepository.GetLastFivePassword(body.Id);
                    Argon2Wrapper wrapper = new Argon2Wrapper();
                    foreach (string password in lastFivePasswords)
                    {
                        if (wrapper.VerifyPassword(password, body.Password))
                        {
                            result = new BadRequestObjectResult(new { error = "You need to enter a password that has not been used the last 5 times" });
                            return result;
                        }
                    }
                    string hashedPassword = wrapper.HashPassword(body.Password);
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
        #endregion
    }
}