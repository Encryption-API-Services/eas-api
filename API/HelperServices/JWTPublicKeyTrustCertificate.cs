using CasDotnetSdk.DigitalSignature.Types;
using CasDotnetSdk.DigitalSignature;
using DataLayer.Redis;
using System.Text.Json;
using CASHelpers;

namespace API.HelperServices
{
    public class JWTPublicKeyTrustCertificate : IJWTPublicKeyTrustCertificate
    {
        private readonly IRedisClient _redisClient;
        public JWTPublicKeyTrustCertificate(IRedisClient redisClient)
        {
            this._redisClient = redisClient;
        }

        public void CreatePublicKeyTrustCertificate(string publicKey, string userId)
        {
            SHA512DigitalSignatureWrapper dsWrapper = new SHA512DigitalSignatureWrapper();
            SHAED25519DalekDigitialSignatureResult ds = dsWrapper.CreateED25519(Convert.FromBase64String(publicKey));
            string dsCacheKey = Constants.RedisKeys.JWTPublicKeySignature + userId;
            this._redisClient.SetString(dsCacheKey, JsonSerializer.Serialize(ds), new TimeSpan(1, 0, 0));
        }
    }
}
