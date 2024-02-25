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
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler().ReadJwtToken(token);
                string publicKey = handler.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                string userId = handler.Claims.First(x => x.Type == Constants.TokenClaims.Id).Value;
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
                await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("You did not supply a token."));
            }
        }
    }
}
