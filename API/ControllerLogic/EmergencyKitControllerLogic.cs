using CasDotnetSdk.Hybrid;
using CasDotnetSdk.PasswordHashers;
using Common;
using Common.UniqueIdentifiers;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using DataLayer.RabbitMQ.QueueMessages;
using Microsoft.AspNetCore.Mvc;
using Models.EmergencyKit;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace API.ControllerLogic
{
    public class EmergencyKitControllerLogic : IEmergencyKitControllerLogic
    {
        private readonly ICASExceptionRepository _exceptionRepostory;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly IUserRepository _userRepository;
        private readonly EmergencyKitRecoveryPublish _emgerencyKitRecovery;
        public EmergencyKitControllerLogic(
             ICASExceptionRepository exceptionRepostory,
             BenchmarkMethodCache benchMarkMethodCache,
             IUserRepository userRepository,
             EmergencyKitRecoveryPublish emgerencyKitRecovery
            )
        {
            this._exceptionRepostory = exceptionRepostory;
            this._benchMarkMethodCache = benchMarkMethodCache;
            this._userRepository = userRepository;
            this._emgerencyKitRecovery = emgerencyKitRecovery;
        }

        public async Task<IActionResult> RecoverProfile(HttpContext context, EmgerencyKitRecoverProfileRequest request)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            try
            {
                // TODO: email regex is acting up, put in validation of a valid email
                User user = await this._userRepository.GetUserByEmail(request.Email);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { error = "No user matches that email address" });
                }
                byte[] secretKey = Convert.FromBase64String(request.Secret);
                HpkeWrapper hpke = new HpkeWrapper();
                byte[] decrypted = hpke.Decrypt(user.EmergencyKit.CipherText, user.EmergencyKit.PrivateKey, secretKey, user.EmergencyKit.Tag, user.EmergencyKit.InfoStr);
                string decryptedGuid = Encoding.UTF8.GetString(decrypted);
                if (decryptedGuid == user.EmergencyKit.EmergencyKitId)
                {
                    string newPassword = new Generator().GenerateRandomPassword(12);
                    Argon2Wrapper argon2 = new Argon2Wrapper();
                    string hashedNewPassword = argon2.HashPassword(newPassword);
                    await this._userRepository.UpdatePassword(user.Id, hashedNewPassword);
                    EmergencyKitRecoveryQueueMessage newMessage = new EmergencyKitRecoveryQueueMessage()
                    {
                        NewPassword = newPassword,
                        UserEmail = user.Email
                    };
                    this._emgerencyKitRecovery.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newMessage)));
                    await this._userRepository.UnlockUser(user.Id);
                    return new OkResult();
                }
                else
                {
                    return new BadRequestObjectResult(new { error = "You did not pass us a correct key, the decrypted key doesn't match." });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepostory.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                return new BadRequestObjectResult(new { error = "There was an error on our end recovering your account" });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return new OkResult();
        }
    }
}
