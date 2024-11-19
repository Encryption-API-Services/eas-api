using CASHelpers;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Models.UserAdmin;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reflection;

namespace API.ControllerLogic
{
    public class UserAdminControllerLogic : IUserAdminControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkCache;
        private readonly IRedisClient _redisClient;
        public UserAdminControllerLogic(
            IUserRepository userRepository,
            ICASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache,
            IRedisClient redisClient
            )
        {
            this._userRepository = userRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkCache = benchmarkMethodCache;
            this._redisClient = redisClient;
        }

        public async Task<IActionResult> DeleteUser(HttpContext context, UserAdminDeleteUserRequest request)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                await this._userRepository.DeleteUserByUserId(request.UserId);
                string isUserActiveRedisKey = Constants.RedisKeys.IsActiveUser + request.UserId;
                this._redisClient.SetString(isUserActiveRedisKey, false.ToString(), new TimeSpan(1, 0, 0));
                result = new OkResult();
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our deleting the user." });
            }
            logger.EndExecution();
            this._benchMarkCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> GetUsers(HttpContext context, int pageSkip, int pageSize)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                IQueryable<UserTableItem> users = this._userRepository.GetUsersByPage();
                Task<int> usersCount = users.CountAsync();
                Task<List<UserTableItem>> usersList = users.Skip(pageSkip * pageSize).Take(pageSize).ToListAsync();
                await Task.WhenAll(usersCount, usersList);
                result = new OkObjectResult(new { Count = usersCount.Result, UserTableItems = usersList.Result });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end getting the user list." });
            }
            logger.EndExecution();
            this._benchMarkCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> UserActivationStatus(HttpContext httpContext, UserActivationStatusRequest request)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                await this._userRepository.ChangeUserActivationStatusById(request.UserId, request.IsActive);
                if (!request.IsActive)
                {
                    string isUserActiveRedisKey = Constants.RedisKeys.IsActiveUser + request.UserId;
                    this._redisClient.SetString(isUserActiveRedisKey, false.ToString(), new TimeSpan(1, 0, 0));
                }
                result = new OkResult();
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end changing the user activation status" });
            }
            logger.EndExecution();
            this._benchMarkCache.AddLog(logger);
            return result;
        }

        public async Task<IActionResult> UserAdminStatus(HttpContext context, UserAdminStatusRequest request)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                await this._userRepository.ChangeUserAdminStatusById(request.UserId, request.IsAdmin);
                string redisString = Constants.RedisKeys.IsUserAdmin + request.UserId;
                this._redisClient.SetString(redisString, request.IsAdmin.ToString(), new TimeSpan(1, 0, 0));
                result = new OkResult();
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "There was an error on our end changing the user admin status" });
            }
            logger.EndExecution();
            this._benchMarkCache.AddLog(logger);
            return result;
        }
    }
}
