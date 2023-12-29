using CASHelpers;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Models.TrialPeriod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Middleware
{
    public class IsUserSubscribedMiddleware
    {
        private readonly int _trialPeriodRequestsLimit = 1000;
        private readonly RequestDelegate _next;
        private readonly List<string> _routesToValidate;

        public IsUserSubscribedMiddleware(RequestDelegate next)
        {
            this._next = next;
            this._routesToValidate = this.RoutesToValidate();
        }
        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository, ITrialPeriodRepository trialPeriodRepository)
        {
            string routePath = context.Request.Path;
            if (this._routesToValidate.BinarySearch(routePath) > -1)
            {
                string userId = context.Items[Constants.HttpItems.UserID].ToString();
                Task<long> trialPeriodRequestsCount = trialPeriodRepository.GetTrialPeriodRequestsCount(userId);
                Task<User> user = userRepository.GetUserById(userId);
                await Task.WhenAll(trialPeriodRequestsCount, user);
                {
                    if (user.Result.UserSubscriptionSettings.IsSubscribed == true)
                    {
                        await this._next(context);
                    }
                    else if (user.Result.UserSubscriptionSettings.IsSubscribed == false && user.Result.UserSubscriptionSettings.HasTrialPeriodExpired == false && trialPeriodRequestsCount.Result < this._trialPeriodRequestsLimit)
                    {
                        // Log the request for the trial period.
                        TrialPeriodRequest trialPeriodRequest = new TrialPeriodRequest()
                        {
                            UserId = userId,
                            CreateTime = DateTime.UtcNow,
                            Route = routePath
                        };
                        await trialPeriodRepository.Insert(trialPeriodRequest);
                        if (trialPeriodRequestsCount.Result + 1 >= this._trialPeriodRequestsLimit)
                        {
                            await userRepository.UpdateTrialPeriodToExpired(userId);
                        }
                        await this._next(context);
                    }
                    else if (user.Result.UserSubscriptionSettings.IsSubscribed == false && user.Result.UserSubscriptionSettings.HasTrialPeriodExpired == true && trialPeriodRequestsCount.Result >= this._trialPeriodRequestsLimit)
                    {
                        // Status Code of Payment Required
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status#client_error_responses
                        context.Response.StatusCode = 402;
                        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Your trial period has ended. You must enter your credit card information on our site."));
                    }
                }
            }
            else
            {
                await this._next(context);
            }
        }

        private List<string> RoutesToValidate()
        {
            return new List<string>()
            {
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
                "/Password/BCryptEncrypt",
                "/Password/BcryptVerify",
                "/Password/BCryptEncryptBatch",
                "/Password/SCryptEncrypt",
                "/Password/SCryptEncryptBatch",
                "/Password/SCryptVerify",
                "/Password/Argon2Hash",
                "/Password/Argon2HashBatch",
                "/Password/Argon2Verify",
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
                "/Signature/Blake2ED25519DalekVerify"
            }.OrderBy(x => x).ToList();
        }
    }
}
