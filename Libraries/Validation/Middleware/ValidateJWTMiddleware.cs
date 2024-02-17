using CASHelpers;
using DataLayer.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Middleware
{
    public class ValidateJWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDatabaseSettings _settings;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<string> _routesToValidate;

        public ValidateJWTMiddleware(RequestDelegate next, IDatabaseSettings databaseSettings)
        {
            _next = next;
            this._settings = databaseSettings;
            this._routesToValidate = this.RoutesToValidate();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
            string routePath = context.Request.Path;
            if (token != null && (this._routesToValidate.BinarySearch(routePath) > -1))
            {
                var handler = new JwtSecurityTokenHandler().ReadJwtToken(token);
                string publicKey = handler.Claims.First(x => x.Type == Constants.TokenClaims.PublicKey).Value;
                string userId = handler.Claims.First(x => x.Type == Constants.TokenClaims.Id).Value;
                context.Items[Constants.HttpItems.UserID] = userId;
                ECDSAWrapper ecdsa = new ECDSAWrapper("ES521");
                ecdsa.ImportFromPublicBase64String(publicKey);
                // validate signing key
                if (await new JWT().ValidateECCToken(token, ecdsa.ECDKey))
                {
                    // proceed to route logic that the JWT is actually protecting.
                    await _next(context);
                }
                else
                {
                    // Status Code of Unauthorized
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status#client_error_responses
                    context.Response.StatusCode = 401;
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Your token has expired. Please autheticate with a refreshed or new token."));
                }
            }
            else
            {
                await _next(context);
            }
        }
        private List<string> RoutesToValidate()
        {
            return new List<string>()
            {
                "/ApiKey/RegenerateApiKey",
                "/BenchmarkSDKMethod/MethodBenchmark",
                "/BenchmarkSDKMethod/GetUserBenchmarksByDays",
                "/Encryption/EncryptAES",
                "/Encryption/DecryptAES",
                "/Encryption/EncryptSHA256",
                "/Encryption/EncryptSHA512",
                "/Encryption/EncryptAESRSAHybrid",
                "/Encryption/DecryptAESRSAHybrid",
                "/Encryption/HashMD5",
                "/Encryption/VerifyMD5",
                "/Encryption/Blake2",
                "/Encryption/Blake2Verify",
                "/Credit/ValidateCard",
                "/Password/BCryptEncrypt",
                "/Password/BcryptVerify",
                "/Password/BCryptEncryptBatch",
                "/Password/SCryptEncrypt",
                "/Password/SCryptEncryptBatch",
                "/Password/SCryptVerify",
                "/Password/Argon2Hash",
                "/Password/Argon2HashBatch",
                "/Password/Argon2Verify",
                "/TwoFA/Get2FAStatus",
                "/TwoFA/TurnOn2FA",
                "/TwoFA/TurnOff2FA",
                "/TwoFA/UpdatePhoneNumber",
                "/UserLogin/GetSuccessfulLogins",
                "/UserLogin/WasLoginMe",
                "/UserLogin/GetApiKey",
                "/Credit/AddCreditCard",
                "/Blog/CreatePost",
                "/Blog/UpdatePost",
                "/Blog/DeletePost",
                "/Blog",
                "/Rsa/GetKeyPair",
                "/Rsa/EncryptWithoutPublic",
                "/Rsa/EncryptWithPublic",
                "/Rsa/Decrypt",
                "/Rsa/DecryptWithStoredPrivate",
                "/Rsa/SignWithoutKey",
                "/Rsa/SignWithKey",
                "/Rsa/Verify",
                "/ED25519/KeyPair",
                "/ED25519/SignWithKeyPair",
                "/ED25519/VerifyWithPublicKey",
                "/Signature/SHA512SignedRSA",
                "/Signature/SHA512SignedRSAVerify",
                "/Signature/SHA512ED25519DalekSign",
                "/Signature/SHA512ED25519DalekVerify",
                "/Signature/HMACSign",
                "/Signature/HMACVerify",
                "/Signature/Blake2RsaSign",
                "/Signature/Blake2RsaVerify",
                "/Signature/Blake2ED25519DalekSign",
                "/Signature/Blake2ED25519DalekVerify",
                "/UserRegister/DeleteUser",
                "/UserSettings/Username",
                "/UserSettings/Password",
                "/Payments/CreateProduct",
                "/Payments/GetProducts",
                "/Payments/CreatePrice",
                "/Payments/GetProductsWithPrice",
                "/Payments/AssignProductToUser",
                "/Payments/DisableSubscription",
                "/Payments/GetBillingInformation",
                "/Payments/UpdateBillingInformation",
            }.OrderBy(x => x).ToList();
        }
    }

    public static class ValidateJWTMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateJWTMiddleware>();
        }
    }
}
