using CASHelpers;
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
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        public AuthenticationControllerLogic(
            IRedisClient redisClient,
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache
            )
        {
            this._redisClient = redisClient;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
        }

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
                    if (cacheInformation.IP != httpContext.Request.HttpContext.Items[Constants.HttpItems.IP].ToString())
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
                    OSInfoRedisEntry newEntry = new OSInfoRedisEntry()
                    {
                        IP = httpContext.Request.HttpContext.Items[Constants.HttpItems.IP].ToString(),
                        OperatingSystem = body.OperatingSystem
                    };
                    string newEntrySeralized = JsonSerializer.Serialize(newEntry);
                    this._redisClient.SetString(cacheKey, newEntrySeralized);
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
    }
}
