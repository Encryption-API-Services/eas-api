using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Drawing.Printing;
using System.Reflection;

namespace API.ControllerLogic
{
    public class UserAdminControllerLogic : IUserAdminControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkCache;
        public UserAdminControllerLogic(
            IUserRepository userRepository, 
            ICASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchmarkMethodCache
            )
        {
            this._userRepository = userRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkCache = benchmarkMethodCache;
        }
        public async Task<IActionResult> GetUsers(HttpContext context, int pageSkip, int pageSize)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                IFindFluent<User, User> users = this._userRepository.GetUsersByPage();
                Task<long> usersCount = users.CountDocumentsAsync();
                Task<List<User>> usersList = users.Skip(pageSkip * pageSize).Limit(pageSize).ToListAsync();
                await Task.WhenAll(usersCount, usersList);
                result = new OkObjectResult(new { Count = usersCount, UserList = usersList });
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
    }
}
