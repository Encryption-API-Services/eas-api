using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;

namespace Email_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EmailServiceInfiscalEnvironment.SetEnvironmentKeys();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "CAS-Email-Service";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
                    MongoClient client = new MongoClient(settings);
                    services.AddSingleton<IMongoClient, MongoClient>(s =>
                    {
                        return client;
                    });
                    services.AddSingleton<IDatabaseSettings, DatabaseSettings>();
                    services.AddSingleton<RabbitMQConnection>();
                    services.AddScoped<ActivateUserQueueSubscribe>();
                    services.AddScoped<ForgotPasswordQueueSubscribe>();
                    services.AddScoped<LockedOutUserQueueSubscribe>();
                    services.AddScoped<CreditCardInformationChangedQueueSubscribe>();
                    services.AddScoped<Email2FAHotpCodeQueueSubscribe>();
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddHostedService<Worker>();
                });
    }
}
