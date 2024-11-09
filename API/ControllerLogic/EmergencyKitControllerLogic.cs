using CasDotnetSdk.Hybrid;
using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.EmergencyKit;
using System.Reflection;
using System.Text;
using System.Web;
using Validation;

namespace API.ControllerLogic
{
    public class EmergencyKitControllerLogic : IEmergencyKitControllerLogic
    {
        private readonly ICASExceptionRepository _exceptionRepostory;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly IUserRepository _userRepository;
        public EmergencyKitControllerLogic(
             ICASExceptionRepository exceptionRepostory,
             BenchmarkMethodCache benchMarkMethodCache,
             IUserRepository userRepository 
            )
        {
            this._exceptionRepostory = exceptionRepostory;
            this._benchMarkMethodCache = benchMarkMethodCache;
            this._userRepository = userRepository;
        }

        public async Task<IActionResult> RecoverProfile(HttpContext context, EmgerencyKitRecoverProfileRequest request)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            try
            {
                // TODO: email regex is acting up, put in validation of a valid email
                EmergencyKit storedKit = await this._userRepository.GetEmergencyKitByEmail(request.Email);
                if (storedKit == null)
                {
                    return new BadRequestObjectResult(new { error = "No user matches that email address" });
                }
                byte[] secretKey = Convert.FromBase64String(request.Secret);
                HpkeWrapper hpke = new HpkeWrapper();
                byte[] decrypted = hpke.Decrypt(storedKit.CipherText, storedKit.PrivateKey, secretKey, storedKit.Tag, storedKit.InfoStr);
                string decryptedGuid = Encoding.UTF8.GetString(decrypted);
                if (decryptedGuid == storedKit.EmergencyKitId)
                {
                    Console.WriteLine("welcome home");
                }
                else
                {
                    // TODO: Send invalid response that the key is not valid.
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
