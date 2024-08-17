using CASHelpers;
using CASHelpers.Types.HttpResponses.UserAuthentication;
using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using DataLayer.Redis;
using Microsoft.AspNetCore.Mvc;
using Models.UserAuthentication;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace API.ControllerLogic
{
    public class TokenControllerLogic : ITokenControllerLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly ICASExceptionRepository _exceptionRepository;
        private readonly BenchmarkMethodCache _benchMarkMethodCache;
        private readonly IRedisClient _redisClient;
        public TokenControllerLogic(
            IUserRepository userRepository,
            ICASExceptionRepository exceptionRepository,
            BenchmarkMethodCache benchMarkMethodCache,
            IRedisClient redisClient
            )
        {
            this._userRepository = userRepository;
            this._exceptionRepository = exceptionRepository;
            this._benchMarkMethodCache = benchMarkMethodCache;
            this._redisClient = redisClient;
        }

        #region GetToken
        public async Task<IActionResult> GetToken(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                string apiKey = httpContext.Request.Headers[Constants.HeaderNames.ApiKey].ToString();
                if (string.IsNullOrEmpty(apiKey))
                {
                    result = new UnauthorizedObjectResult(new { error = "You did not set an ApiKey" });
                }
                User activeUser = await this._userRepository.GetUserByApiKey(apiKey);
                if (activeUser == null)
                {
                    result = new UnauthorizedObjectResult(new { error = "You entered an invalid ApiKey" });
                }
                else if (activeUser.IsActive == false)
                {
                    result = new UnauthorizedObjectResult(new { error = "This user account is no longer active" });
                }
                else
                {
                    ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                    string token = new JWT().GenerateECCToken(activeUser.Id, activeUser.IsAdmin, ecdsa, 1, activeUser.StripProductId);
                    string isUserActiveRedisKey = Constants.RedisKeys.IsActiveUser + activeUser.Id;
                    this._redisClient.SetString(isUserActiveRedisKey, true.ToString(), new TimeSpan(1, 0, 0));
                    result = new OkObjectResult(new GetTokenResponse() { Token = token });
                }
                return result;

            }
            catch (Exception ex)
            {
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end" });
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region GetRefreshToken
        public async Task<IActionResult> GetRefreshToken(HttpContext context)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(context);
            IActionResult result = null;
            try
            {
                // get current token
                string token = context.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                if (!string.IsNullOrEmpty(token) && handler.CanReadToken(token))
                {
                    JwtSecurityToken parsedToken = handler.ReadJwtToken(token);
                    string publicKey = parsedToken.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                    ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                    ecdsa.ImportFromPublicBase64String(publicKey);
                    JWT jwtWrapper = new JWT();
                    if (await jwtWrapper.ValidateECCToken(token, ecdsa.ECDKey))
                    {
                        ECDSAWrapper newEcdsa = new ECDSAWrapper("ES521");
                        string userId = jwtWrapper.GetUserIdFromToken(token);
                        bool isAdmin = bool.Parse(parsedToken.Claims.First(x => x.Type == Constants.TokenClaims.IsAdmin).Value);
                        string stripProductId = parsedToken.Claims.First(x => x.Type == Constants.TokenClaims.SubscriptionPublicKey).Value;
                        string newToken = new JWT().GenerateECCToken(userId, isAdmin, newEcdsa, 1, stripProductId);
                        string isUserActiveRedisKey = Constants.RedisKeys.IsActiveUser + userId;
                        this._redisClient.SetString(isUserActiveRedisKey, true.ToString(), new TimeSpan(1, 0, 0));
                        result = new OkObjectResult(new GetTokenResponse() { Token = newToken });
                    }
                    else
                    {
                        // if token is not valid send them unauthorized response
                        result = new UnauthorizedResult();
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                // TODO: give error message
                result = new BadRequestObjectResult(new { });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }

        #endregion

        #region
        /// <summary>
        /// This route is not included in the authentication middleware and we validate the token within this request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> IsTokenValid(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = new OkObjectResult(new IsTokenValidResponse() { IsTokenValid = true });
            try
            {
                var token = httpContext.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                if (string.IsNullOrEmpty(token) && jwtHandler.CanReadToken(token))
                {
                    result = new OkObjectResult(new IsTokenValidResponse() { IsTokenValid = false });
                }
                else
                {
                    JwtSecurityToken parsedToken = jwtHandler.ReadJwtToken(token);
                    string publicKey = parsedToken.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                    ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                    ecdsa.ImportFromPublicBase64String(publicKey);
                    // validate signing key
                    if (!await new JWT().ValidateECCToken(token, ecdsa.ECDKey))
                    {
                        result = new OkObjectResult(new IsTokenValidResponse() { IsTokenValid = false });
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new IsTokenValidResponse() { IsTokenValid = false });
            }
            logger.EndExecution();
            this._benchMarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}