using CasDotnetSdk.KeyExchange;
using CasDotnetSdk.KeyExchange.Types;
using CasDotnetSdk.Symmetric;
using CasDotnetSdk.Symmetric.Types;
using CASHelpers;
using CASHelpers.Types.HttpResponses.UserAuthentication;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication.AuthenticationController;
using System.Reflection;
using System.Text.Json;

namespace API.ControllerLogic
{
    public class AuthenticationControllerLogic : IAuthenticationControllerLogic
    {
        private readonly IRedisClient _redisClient;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        public AuthenticationControllerLogic(
            IRedisClient redisClient,
            ICASExceptionRepository exceptionRepository,
            IUserRepository userRepository,
            BenchmarkMethodCache benchMarkMethodCache
            )
        {
            this._redisClient = redisClient;
            this._exceptionRepository = exceptionRepository;
            this._userRepository = userRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
        }

        public async Task<IActionResult> DiffieHellmanAesKeyDerviationForSDK(HttpContext httpContext, DiffieHellmanAesDerivationRequest body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string userId = httpContext.Request.HttpContext.Items[Constants.HttpItems.UserID].ToString();
                string redisKey = Constants.RedisKeys.DiffieHellmanAesKey + userId + "-" + body.MacAddress;
                X25519Wrapper x25519 = new X25519Wrapper();
                X25519SecretPublicKey secretPublicKey = x25519.GenerateSecretAndPublicKey();
                X25519SharedSecret sharedSecret = x25519.GenerateSharedSecret(secretPublicKey.SecretKey, body.RequestersPublicKey);
                AESWrapper aes = new AESWrapper();
                Aes256KeyAndNonceX25519DiffieHellman aesKey = aes.Aes256KeyNonceX25519DiffieHellman(sharedSecret.SharedSecret);
                string serializedAesKey = JsonSerializer.Serialize(aesKey);
                this._redisClient.SetString(redisKey, serializedAesKey);
                result = new OkObjectResult(new DiffieHellmanAesDerivationResponse() { ResponsersPublicKey = secretPublicKey.PublicKey });
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

        #region Remove Operating System Information In Cache
        public async Task<IActionResult> RemoveOperatingSystemInformationInCache(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string requestingUserId = httpContext.Request.HttpContext.Items[Constants.HttpItems.UserID].ToString();
                string cacheKey = Constants.RedisKeys.OsInformation + requestingUserId;
                await this._redisClient.GetDelete(cacheKey);
                result = new OkResult();
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

        #region Store Operating System Information In Cache
        public async Task<IActionResult> StoreOperatingSystemInformationInCache(HttpContext httpContext, OperatingSystemInformationCacheRequestBody body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string requestingUserId = httpContext.Request.HttpContext.Items[Constants.HttpItems.UserID].ToString();
                // check if cache is populated
                string cacheKey = Constants.RedisKeys.OsInformation + requestingUserId;
                string existingCacheInformation = this._redisClient.GetString(cacheKey);
                if (existingCacheInformation != null)
                {
                    JsonSerializerOptions options = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = false,
                    };
                    OSInfoRedisEntry cacheInformation = JsonSerializer.Deserialize<OSInfoRedisEntry>(existingCacheInformation, options);

                    // TODO: perform other checks besides IP address and Operating System
                    // also perform check based upon the API key.
                    if (!cacheInformation.IsApiKeyProd)
                    {
                        result = new OkObjectResult(new { message = "Enjoy using your development key" });
                    }
                    else if (cacheInformation.IP != httpContext.Request.HttpContext.Items[Constants.HttpItems.IP].ToString())
                    {
                        result = new BadRequestObjectResult(new { error = "There is already a UserID using this API Key is associated IP address" });
                    }
                    else if (cacheInformation.OperatingSystem != body.OperatingSystem)
                    {
                        result = new BadRequestObjectResult(new { error = "There is already a UserID using this API Key with this Operating System" });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = "You have reached the maximum amount of User's for this API Key" });
                    }
                }
                else
                {
                    Tuple<string, string> apiKeys = await this._userRepository.GetApiKeysById(requestingUserId);
                    if (apiKeys.Item1 == body.ApiKey)
                    {
                        OSInfoRedisEntry newEntry = new OSInfoRedisEntry()
                        {
                            IP = httpContext.Request.HttpContext.Items[Constants.HttpItems.IP].ToString(),
                            OperatingSystem = body.OperatingSystem,
                            ApiKey = body.ApiKey,
                            IsApiKeyProd = true,
                        };
                        string newEntrySeralized = JsonSerializer.Serialize(newEntry);
                        this._redisClient.SetString(cacheKey, newEntrySeralized);
                    }
                    else if (apiKeys.Item2 == body.ApiKey)
                    {

                        OSInfoRedisEntry newEntry = new OSInfoRedisEntry()
                        {
                            IP = httpContext.Request.HttpContext.Items[Constants.HttpItems.IP].ToString(),
                            OperatingSystem = body.OperatingSystem,
                            ApiKey = body.ApiKey,
                            IsApiKeyProd = false,
                        };
                        string newEntrySeralized = JsonSerializer.Serialize(newEntry);
                        this._redisClient.SetString(cacheKey, newEntrySeralized);
                    }
                    else
                    {
                        result = new UnauthorizedObjectResult(new { message = "You supplied an invalid API key" });
                    }

                    result = new OkObjectResult(new { message = "Sucessfully stored information in cache" });
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
    }
}