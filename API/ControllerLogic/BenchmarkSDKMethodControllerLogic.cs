using CasDotnetSdk.Symmetric;
using CasDotnetSdk.Symmetric.Types;
using CASHelpers;
using CASHelpers.Types.HttpResponses.BenchmarkAPI;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Models.BenchmarkSDKSend;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace API.ControllerLogic
{
    public class BenchmarkSDKMethodControllerLogic : IBenchmarkSDKMethodControllerLogic
    {
        private readonly IBenchmarkSDKMethodRepository _benchmarkSDKMethodRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly IRedisClient _redisClient;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        public BenchmarkSDKMethodControllerLogic(
            IBenchmarkSDKMethodRepository benchmarkSDKMethodRepository,
            ICASExceptionRepository exceptionRepository,
            IRedisClient redisClient,
            BenchmarkMethodCache benchmarkMethodCache)
        {
            this._benchmarkSDKMethodRepository = benchmarkSDKMethodRepository;
            this._exceptionRepository = exceptionRepository;
            this._redisClient = redisClient;
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

        public async Task<IActionResult> CreateMethodSDKBenchmark(BenchmarkMacAddressSDKMethod sdkMethod, HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                string redisKey = Constants.RedisKeys.DiffieHellmanAesKey + userId + "-" + sdkMethod.MacAddress;
                string redisContent = this._redisClient.GetString(redisKey);
                Aes256KeyAndNonceX25519DiffieHellman aesKey = JsonSerializer.Deserialize<Aes256KeyAndNonceX25519DiffieHellman>(redisContent);
                AESWrapper aes = new AESWrapper();
                string decrypted = Encoding.UTF8.GetString(aes.Aes256DecryptBytes(aesKey.AesNonce, aesKey.AesKey, sdkMethod.EncryptedBenchMarkSend));
                BenchmarkSDKMethod unencryptedSdkMethod = JsonSerializer.Deserialize<BenchmarkSDKMethod>(decrypted);
                DataLayer.Mongo.Entities.BenchmarkSDKMethod newBenchmarkMethod = new DataLayer.Mongo.Entities.BenchmarkSDKMethod()
                {
                    MethodDescription = unencryptedSdkMethod?.MethodDescription,
                    MethodName = unencryptedSdkMethod.MethodName,
                    MethodStart = unencryptedSdkMethod.MethodStart,
                    MethodEnd = unencryptedSdkMethod.MethodEnd,
                    MethodType = unencryptedSdkMethod.MethodType,
                    CreatedBy = userId
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
