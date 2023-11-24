using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.TwoFactorAuthentication;
using System.Reflection;
using Validation.Phone;

namespace API.ControllersLogic
{
    public class TwoFAControllerLogic : ITwoFAControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        public TwoFAControllerLogic(
            IUserRepository userRepository,
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache)
        {
            this._userRepository = userRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchmarkMethodCache;
        }

        public async Task<IActionResult> Get2FAStatus(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string userId = httpContext.Items["UserID"].ToString();
                Phone2FA status = await this._userRepository.GetPhone2FAStats(userId);
                result = new OkObjectResult(new { result = status.IsEnabled });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> PhoneNumberUpdate(UpdatePhoneNumber body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string userId = httpContext.Items["UserID"].ToString();
                PhoneValidator phoneValidator = new PhoneValidator();
                if (phoneValidator.IsPhoneNumberValid(body.PhoneNumber))
                {
                    await this._userRepository.ChangePhoneNumberByUserID(userId, body.PhoneNumber);
                    result = new OkObjectResult(new { message = "You have successfully change your phone number for 2FA" });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "You did not provide a valid phone number" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> TurnOff2FA(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string userId = httpContext.Items["UserID"].ToString();
                await this._userRepository.ChangePhone2FAStatusToDisabled(userId);
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> TurnOn2FA(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string userId = httpContext.Items["UserID"].ToString();
                await this._userRepository.ChangePhone2FAStatusToEnabled(userId);
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end." });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
    }
}
