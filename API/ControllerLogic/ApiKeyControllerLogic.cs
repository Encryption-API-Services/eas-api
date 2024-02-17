using Common;
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
        public ApiKeyControllerLogic(
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache
            )
        {
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
        }

        public async Task<IActionResult> RegenerateApiKey(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                
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