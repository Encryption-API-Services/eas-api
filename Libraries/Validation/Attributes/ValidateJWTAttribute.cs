using CASHelpers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Validation.Attributes
{
    public sealed class ValidateJWTAttribute : Attribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (token != null && handler.CanReadToken(token))
            {
                var readToken = handler.ReadJwtToken(token);
                string publicKey = readToken.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                string userId = readToken.Claims.First(x => x.Type == Constants.TokenClaims.Id).Value;
                // TODO: check that the user is active
                context.HttpContext.Items[Constants.HttpItems.UserID] = userId;
                ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                ecdsa.ImportFromPublicBase64String(publicKey);
                // validate signing key
                if (!await new JWT().ValidateECCToken(token, ecdsa.ECDKey))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Your token has expired. Please autheticate with a refreshed or new token."));
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("You did not supply a token or it is malformed."));
            }
        }
    }
}
