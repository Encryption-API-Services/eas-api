using API.ControllersLogic;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Encryption.PasswordHash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.UserAuthentication;
using Payments;
using System.Reflection;
using System.Runtime.InteropServices;
using Validation.UserRegistration;
using User = DataLayer.Mongo.Entities.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Config
{
    public class UserRegisterControllerLogic : IUserRegisterControllerLogic
    {
        private readonly IUserRepository _userRespository;
        private readonly ILogRequestRepository _logRequestRespository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        public UserRegisterControllerLogic(
            IUserRepository userRepo,
            ILogRequestRepository logRequestRespository,
            IEASExceptionRepository exceptionRespitory,
            BenchmarkMethodCache benchmarkMethodCache
            )
        {
            this._userRespository = userRepo;
            this._logRequestRespository = logRequestRespository;
            this._exceptionRepository = exceptionRespitory;
            this._benchmarkMethodCache = benchmarkMethodCache;
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
                    Argon2Wrappper argon2 = new Argon2Wrappper();
                    IntPtr hashedPasswordPtr = await argon2.HashPasswordAsync(body.password);
                    string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
                    await this._userRespository.AddUser(body, hashedPassword);
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
                string signature = Base64UrlEncoder.Decode(body.Token);
                RustRSAWrapper rustRsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                bool isValid = rustRsaWrapper.RsaVerify(userToActivate.EmailActivationToken.PublicKey, userToActivate.EmailActivationToken.Token, signature);
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
                string signature = Base64UrlEncoder.Decode(body.Token);
                RustRSAWrapper rustRsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
                bool isValid = rustRsaWrapper.RsaVerify(user.InactiveEmail.PublicKey, user.InactiveEmail.Token, signature);
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
                string userId = context.Items["UserID"].ToString();
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