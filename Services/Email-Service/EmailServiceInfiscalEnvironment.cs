using DataLayer.Infiscial;
using System;

namespace Email_Service
{
    internal class EmailServiceInfiscalEnvironment
    {
        public static void SetEnvironmentKeys()
        {
            Environment.SetEnvironmentVariable("Connection", InfiscialEnvironment.GetSecretFromStorage("CONNECTION"));
            Environment.SetEnvironmentVariable("DatabaseName", InfiscialEnvironment.GetSecretFromStorage("DATABASENAME"));
            Environment.SetEnvironmentVariable("RabbitMqUrl", InfiscialEnvironment.GetSecretFromStorage("RABBITMQURL"));
            Environment.SetEnvironmentVariable("UserCollectionName", InfiscialEnvironment.GetSecretFromStorage("USERCOLLECTIONNAME"));
            Environment.SetEnvironmentVariable("Email", InfiscialEnvironment.GetSecretFromStorage("EMAIL"));
            Environment.SetEnvironmentVariable("EmailPass", InfiscialEnvironment.GetSecretFromStorage("EMAILPASS"));
            Environment.SetEnvironmentVariable("Domain", InfiscialEnvironment.GetSecretFromStorage("DOMAIN"));
        }
    }
}
