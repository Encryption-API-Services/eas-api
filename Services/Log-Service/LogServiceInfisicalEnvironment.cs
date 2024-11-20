using DataLayer.Infiscial;

namespace Log_Service
{
    internal class LogServiceInfisicalEnvironment
    {
        public static void Setup()
        {
            Environment.SetEnvironmentVariable("Connection", InfiscialEnvironment.GetSecretFromStorage("CONNECTION"));
            Environment.SetEnvironmentVariable("DatabaseName", InfiscialEnvironment.GetSecretFromStorage("DATABASENAME"));
            Environment.SetEnvironmentVariable("RabbitMqUrl", InfiscialEnvironment.GetSecretFromStorage("RABBITMQURL"));
            Environment.SetEnvironmentVariable("UserCollectionName", InfiscialEnvironment.GetSecretFromStorage("USERCOLLECTIONNAME"));
        }
    }
}
