using DataLayer.Infiscial;

namespace API
{
    public static class APIInfiscialEnvironment
    {
        public static void SetEnvironmentKeys()
        {
            Environment.SetEnvironmentVariable("Connection", InfiscialEnvironment.GetSecretFromStorage("CONNECTION"));
            Environment.SetEnvironmentVariable("DatabaseName", InfiscialEnvironment.GetSecretFromStorage("DATABASENAME"));
            Environment.SetEnvironmentVariable("RabbitMqUrl", InfiscialEnvironment.GetSecretFromStorage("RABBITMQURL"));
            Environment.SetEnvironmentVariable("RedisIp", InfiscialEnvironment.GetSecretFromStorage("REDISIP"));
            Environment.SetEnvironmentVariable("StripApiKey", InfiscialEnvironment.GetSecretFromStorage("STRIPAPIKEY"));
            Environment.SetEnvironmentVariable("UserCollectionName", InfiscialEnvironment.GetSecretFromStorage("USERCOLLECTIONNAME"));
        }
    }
}
