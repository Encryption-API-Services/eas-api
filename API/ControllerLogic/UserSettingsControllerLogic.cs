using CasDotnetSdk.Hashers;
using CasDotnetSdk.Hybrid;
using CasDotnetSdk.Hybrid.Types;
using CasDotnetSdk.PasswordHashers;
using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Encryption.AESRSAHybrid;
using Models.UserAuthentication;
using Models.UserSettings;
using Stripe;
using System.Reflection;
using Validation.UserSettings;

namespace API.ControllerLogic
{
    public class UserSettingsControllerLogic : IUserSettingsControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly UserSettingsValidation _userSettingsValidation;
        public UserSettingsControllerLogic(
            IUserRepository userRepository,
            ICASExceptionRepository exceptionRepository,
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

        #region EmergencyKitRecovery
        public async Task<IActionResult> EmergencyKitRecovery(HttpContext context, EmergencyKitRecoveryBody recoveryBody)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                EmergencyKitValidationResult validationResult = this._userSettingsValidation.IsEmergencyKitValid(recoveryBody);
                if (!validationResult.IsValid)
                {
                    result = new BadRequestObjectResult(new { error = validationResult.ErrorMessage });
                }
                else
                {
                    byte[] decodedAesKey = Convert.FromBase64String(recoveryBody.SecretKey);
                    HybridEncryptionWrapper hybridWrapper = new HybridEncryptionWrapper();
                    AESRSAHybridEncryptResult encryptonResult = validationResult.AccountRecoverySettings.EncryptedResult;
                    encryptonResult.EncryptedAesKey = decodedAesKey;
                    byte[] decryptedCipherText = hybridWrapper.DecryptAESRSAHybrid(validationResult.AccountRecoverySettings.RsaPrivateKey, encryptonResult);
                    SHAWrapper sha = new SHAWrapper();
                    bool isValid = sha.Verify512(decryptedCipherText, Convert.FromBase64String(validationResult.AccountRecoverySettings.Key));
                    if (!isValid)
                    {
                        result = new UnauthorizedObjectResult(new { error = "Your secret key was unable to recover your account, are you sure you copied and pasted it correctly?" });
                    }
                    else
                    {
                         string newPassword = 
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end recovering your account with your secret key" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}