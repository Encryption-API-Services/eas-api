using CASHelpers;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;

namespace Validation.Attributes
{
    public sealed class IsAdminAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IRedisClient _redisClient;
        private readonly IUserRepository _userRepository;
        public IsAdminAttribute(
            IRedisClient redisClient,
            IUserRepository userRepository
            )
        {
            this._userRepository = userRepository;
            this._redisClient = redisClient;
        }
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            string userId = context.HttpContext.Items[Constants.HttpItems.UserID].ToString();
            string redisKey = Constants.RedisKeys.IsUserAdmin + context.HttpContext.Items[Constants.HttpItems.UserID].ToString();
            string isAdmin = this._redisClient.GetString(redisKey);
            bool isAdminBool = true;
            if (string.IsNullOrEmpty(isAdmin) || !bool.TryParse(isAdmin, out isAdminBool))
            {
                User dbUser = await this._userRepository.GetUserById(userId);
                if (dbUser == null || !dbUser.IsAdmin)
                {
                    isAdminBool = false;
                }
                else
                {
                    isAdminBool = true;
                }
                this._redisClient.SetString(redisKey, isAdminBool.ToString(), new TimeSpan(1, 0, 0));
            }
            if (!isAdminBool)
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("You are not an admin."));
                context.Result = new UnauthorizedObjectResult(new { });
            }
        }
    }
}
