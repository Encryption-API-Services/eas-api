using DataLayer.Infiscial;
using DataLayer.Redis;

namespace API
{
    public static class APIInfiscialEnvironment
    {

        public static void Setup()
        {
            SetEnvironmentKeys();
            SetCache();
        }
        private static void SetEnvironmentKeys()
        {
            Environment.SetEnvironmentVariable("Connection", InfiscialEnvironment.GetSecretFromStorage("CONNECTION"));
            Environment.SetEnvironmentVariable("DatabaseName", InfiscialEnvironment.GetSecretFromStorage("DATABASENAME"));
            Environment.SetEnvironmentVariable("RabbitMqUrl", InfiscialEnvironment.GetSecretFromStorage("RABBITMQURL"));
            Environment.SetEnvironmentVariable("RedisIp", InfiscialEnvironment.GetSecretFromStorage("REDISIP"));
            Environment.SetEnvironmentVariable("StripApiKey", InfiscialEnvironment.GetSecretFromStorage("STRIPAPIKEY"));
            Environment.SetEnvironmentVariable("UserCollectionName", InfiscialEnvironment.GetSecretFromStorage("USERCOLLECTIONNAME"));
        }

        private static void SetCache()
        {
            RedisClient redisClient = new RedisClient();
            string aesKey = InfiscialEnvironment.GetSecretFromStorage("PUBLICKEYNONCE");
            string aesNonce = InfiscialEnvironment.GetSecretFromStorage("PUBLICKEYKEY");
            redisClient.SetString("PUBLICKEYNONCE", aesKey, null);
            redisClient.SetString("PUBLICKEYKEY", aesNonce, null);
        }
    }
}
