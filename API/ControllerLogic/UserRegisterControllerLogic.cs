using API.ControllersLogic;
using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.PasswordHashers;
using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using DataLayer.RabbitMQ.QueueMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.UserAuthentication;
using Payments;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Validation.UserRegistration;
using User = DataLayer.Mongo.Entities.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Config
{
    public class UserRegisterControllerLogic : IUserRegisterControllerLogic
    {
        private readonly IUserRepository _userRespository;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly ILogRequestRepository _logRequestRespository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        private readonly ActivateUserQueuePublish _activateUserQueue;
        public UserRegisterControllerLogic(
            IUserRepository userRepo,
            IForgotPasswordRepository forgotPasswordRepository,
            ILogRequestRepository logRequestRespository,
            IEASExceptionRepository exceptionRespitory,
            BenchmarkMethodCache benchmarkMethodCache,
            ActivateUserQueuePublish activateUserQueue
            )
        {
            this._userRespository = userRepo;
            this._forgotPasswordRepository = forgotPasswordRepository;
            this._logRequestRespository = logRequestRespository;
            this._exceptionRepository = exceptionRespitory;
            this._benchmarkMethodCache = benchmarkMethodCache;
            this._activateUserQueue = activateUserQueue;
        }

        #region RegisterUser
        public async Task<IActionResult> RegisterUser(RegisterUser body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {

                RegisterUserValidation validation = new RegisterUserValidation();
                Task<User> emailUser = this._userRespository.GetUserByEmail(body.email);
                Task<User> usernameUser = this._userRespository.GetUserByUsername(body.username);
                await Task.WhenAll(emailUser, usernameUser);
                if (validation.IsRegisterUserModelValid(body) && emailUser.Result == null && usernameUser.Result == null)
                {
                    Argon2Wrapper argon2 = new Argon2Wrapper();
                    string hashedPassword = argon2.HashPassword(body.password);
                    User newUser = await this._userRespository.AddUser(body, hashedPassword);
                    ActivateUserQueueMessage newMessage = new ActivateUserQueueMessage()
                    {
                        Testing = "123456"
                    };
                    this._activateUserQueue.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newMessage)));
                    await this._forgotPasswordRepository.InsertForgotPasswordAttempt(newUser.Id, hashedPassword);
                    result = new OkObjectResult(new { message = "Successfully registered user" });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "This email and or username already exists" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ActivateUser

        public async Task<IActionResult> ActivateUser(ActivateUser body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                User userToActivate = await this._userRespository.GetUserById(body.Id);
                byte[] signature = Base64UrlEncoder.DecodeBytes(body.Token);
                RSAWrapper rustRsaWrapper = new RSAWrapper();
                bool isValid = rustRsaWrapper.RsaVerifyBytes(userToActivate.EmailActivationToken.PublicKey, Convert.FromBase64String(userToActivate.EmailActivationToken.Token), signature);
                if (isValid)
                {
                    StripCustomer stripCustomer = new StripCustomer();
                    string stripCustomerId = await stripCustomer.CreateStripCustomer(userToActivate);
                    await this._userRespository.ChangeUserActiveById(userToActivate, true, stripCustomerId);
                    result = new OkObjectResult(new { message = "User account was successfully activated." });
                }
                else
                {
                    result = new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> InactiveUser(InactiveUser body, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                User user = await this._userRespository.GetUserById(body.Id);
                byte[] signature = Base64UrlEncoder.DecodeBytes(body.Token);
                RSAWrapper rustRsaWrapper = new RSAWrapper();
                bool isValid = rustRsaWrapper.RsaVerifyBytes(user.InactiveEmail.PublicKey, Convert.FromBase64String(user.InactiveEmail.Token), signature);
                if (isValid)
                {
                    // Delete the strip customer and the user account associated with the user id.
                    StripCustomer stripCustomer = new StripCustomer();
                    Task stripDelete = stripCustomer.DeleteStripCustomer(user.StripCustomerId);
                    Task userDelete = this._userRespository.DeleteUserByUserId(user.Id);
                    await Task.WhenAll(stripDelete, userDelete);
                    result = new OkObjectResult(new { });
                }
                else
                {
                    result = new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region DeleteUser
        public async Task<IActionResult> DeleteUser(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                User user = await this._userRespository.GetUserById(userId);
                StripCustomer stripCustomer = new StripCustomer();
                Task deleteCustomer = stripCustomer.DeleteStripCustomer(user.StripCustomerId);
                Task deleteUser = this._userRespository.DeleteUserByUserId(userId);
                await Task.WhenAll(deleteCustomer, deleteUser);
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our side." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}