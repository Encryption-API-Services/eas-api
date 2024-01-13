using CasDotnetSdk.PasswordHashers;
using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;
using System.Reflection;
using System.Runtime.InteropServices;
using Validation.UserSettings;

namespace API.ControllerLogic
{
    public class UserSettingsControllerLogic : IUserSettingsControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly UserSettingsValidation _userSettingsValidation;
        public UserSettingsControllerLogic(
            IUserRepository userRepository,
            IEASExceptionRepository exceptionRepository,
            IForgotPasswordRepository forgotPasswordRepository,
            BenchmarkMethodCache benchmarkMethodCache,
            UserSettingsValidation userSettingsValidation
            )
        {
            this._userRepository = userRepository;
            this._exceptionRepository = exceptionRepository;
            this._forgotPasswordRepository = forgotPasswordRepository;
            this._benchMarkMethodCache = benchmarkMethodCache;
            this._userSettingsValidation = userSettingsValidation;
        }

        #region ChangeUsername
        public async Task<IActionResult> ChangeUsername(HttpContext context, ChangeUserName changeUsername)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                Tuple<bool, string> isValid = await this._userSettingsValidation.IsChangeUsernameValid(changeUsername);
                if (!isValid.Item1)
                {
                    result = new BadRequestObjectResult(new { error = isValid.Item2 });
                }
                else
                {
                    await this._userRepository.ChangeUsername(context.Items[Constants.HttpItems.UserID].ToString(), changeUsername.NewUsername);
                    result = new OkObjectResult(new { message = String.Format("Successfully changed your username to {0}", changeUsername.NewUsername) });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end updating your user name" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region ChangePassword
        public async Task<IActionResult> ChangePassword(HttpContext context, ChangePassword changePassword)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                Argon2Wrapper argon2Wrapper = new Argon2Wrapper();
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                User currentUser = await this._userRepository.GetUserById(userId);
                Argon2Wrapper argon2 = new Argon2Wrapper();
                Tuple<bool, string> isValid = await this._userSettingsValidation.IsChangePasswordValid(changePassword, userId, currentUser.Password, argon2);
                if (!isValid.Item1)
                {
                    result = new BadRequestObjectResult(new { error = isValid.Item2 });
                }
                else
                {
                    await this._forgotPasswordRepository.InsertForgotPasswordAttempt(userId, currentUser.Password);
                    string newPassword = argon2.HashPassword(changePassword.NewPassword);
                    await this._userRepository.UpdatePassword(userId, newPassword);
                    result = new OkObjectResult(new { message = "Successfully changed your password" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end changing your password" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}