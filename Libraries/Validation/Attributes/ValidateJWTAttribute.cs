using CASHelpers;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Validation.Attributes
{
    public sealed class ValidateJWTAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IRedisClient _redisClient;
        private readonly IUserRepository _userRepository;
        public ValidateJWTAttribute(IRedisClient redisClient, IUserRepository userRepository)
        {
            this._redisClient = redisClient;
            this._userRepository = userRepository;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (token != null && handler.CanReadToken(token))
            {
                var readToken = handler.ReadJwtToken(token);
                string publicKey = readToken.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                ecdsa.ImportFromPublicBase64String(publicKey);
                // validate signing key
                if (!await new JWT().ValidateECCToken(token, ecdsa.ECDKey))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Your token has expired. Please autheticate with a refreshed or new token."));
                    context.Result = new UnauthorizedObjectResult(new { });
                }
                else
                {
                    string userId = readToken.Claims.First(x => x.Type == Constants.TokenClaims.Id).Value;
                    string isAdmin = readToken.Claims.First(x => x.Type == Constants.TokenClaims.IsAdmin).Value;
                    string subscriptionProductId = readToken.Claims.First(x => x.Type == Constants.TokenClaims.SubscriptionPublicKey).Value;
                    context.HttpContext.Items[Constants.HttpItems.UserID] = userId;
                    context.HttpContext.Items[Constants.TokenClaims.IsAdmin] = isAdmin;
                    context.HttpContext.Items[Constants.TokenClaims.SubscriptionPublicKey] = subscriptionProductId;

                    // Check that the user is active.
                    string isUserActiveRedisKey = Constants.RedisKeys.IsActiveUser + userId;
                    string isActive = this._redisClient.GetString(isUserActiveRedisKey);

                    if (string.IsNullOrEmpty(isActive))
                    {
                        User dbUser = await this._userRepository.GetUserById(userId);
                        if (dbUser == null || !dbUser.IsActive)
                        {
                            isActive = "False";
                        }
                        else
                        {
                            // if active user set in cache instead of hitting the DB next time.
                            this._redisClient.SetString(isUserActiveRedisKey, true.ToString(), new TimeSpan(1, 0, 0));
                            isActive = "True";
                        }
                    }

                    if (!bool.Parse(isActive))
                    {
                        context.HttpContext.Response.StatusCode = 401;
                        await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("User account is not active."));
                        context.Result = new UnauthorizedObjectResult(new { });
                    }
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("You did not supply a token or it is malformed."));
                context.Result = new UnauthorizedObjectResult(new { });
            }
        }
    }
}
