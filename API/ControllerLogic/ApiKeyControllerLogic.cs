using CASHelpers;
using Common;
using Common.UniqueIdentifiers;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API.ControllerLogic
{
    public class ApiKeyControllerLogic : IApiKeyControllerLogic
    {
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly IUserRepository _userRepository;
        public ApiKeyControllerLogic(
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache,
            IUserRepository userRepository
            )
        {
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
            this._userRepository = userRepository;
        }

        public async Task<IActionResult> RegenerateApiKey(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                Generator generator = new Generator();
                string newApiKey = generator.CreateApiKey();
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                await this._userRepository.UpdateApiKeyByUserId(userId, newApiKey);
                return new OkObjectResult(new { ApiKey = newApiKey });
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
    }
}