using CASHelpers;
using CASHelpers.Types.HttpResponses.BenchmarkAPI;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API.ControllerLogic
{
    public class BenchmarkSDKMethodControllerLogic : IBenchmarkSDKMethodControllerLogic
    {
        private readonly IBenchmarkSDKMethodRepository _benchmarkSDKMethodRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        public BenchmarkSDKMethodControllerLogic(
            IBenchmarkSDKMethodRepository benchmarkSDKMethodRepository,
            IEASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache)
        {
            this._benchmarkSDKMethodRepository = benchmarkSDKMethodRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchmarkMethodCache = benchmarkMethodCache;
        }

        public async Task<IActionResult> GetUserBenchmarksByDays(int daysAgo, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                List<DataLayer.Mongo.Entities.BenchmarkSDKMethod> dbEntities = await this._benchmarkSDKMethodRepository.GetUserBenchmarksDaysAgo(userId, daysAgo);
                List<BenchmarkSDKChartMethod> benchmarks = new List<BenchmarkSDKChartMethod>();
                for (int i = 0; i < dbEntities.Count; i++)
                {
                    BenchmarkSDKChartMethod newBenchmark = new BenchmarkSDKChartMethod()
                    {
                        AmountOfTime = (dbEntities[i].MethodEnd - dbEntities[i].MethodStart).TotalSeconds,
                        MethodDescription = dbEntities[i].MethodDescription,
                        MethodName = dbEntities[i].MethodName,
                        MethodType = dbEntities[i].MethodType
                    };
                    benchmarks.Add(newBenchmark);
                }

                result = new OkObjectResult(new GetUserBenchmarksByDaysResponse() { Benchmarks = benchmarks });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> CreateMethodSDKBenchmark(CASHelpers.Types.HttpResponses.BenchmarkAPI.BenchmarkSDKMethod sdkMethod, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                DataLayer.Mongo.Entities.BenchmarkSDKMethod newBenchmarkMethod = new DataLayer.Mongo.Entities.BenchmarkSDKMethod()
                {
                    MethodDescription = sdkMethod?.MethodDescription,
                    MethodName = sdkMethod.MethodName,
                    MethodStart = sdkMethod.MethodStart,
                    MethodEnd = sdkMethod.MethodEnd,
                    MethodType = sdkMethod.MethodType,
                    CreatedBy = context.Items[Constants.HttpItems.UserID].ToString()
                };
                await this._benchmarkSDKMethodRepository.InsertSDKMethodBenchmark(newBenchmarkMethod);
                result = new OkResult();
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
    }
}
