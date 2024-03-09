using DataLayer.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Email_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                    services.AddSingleton<RabbitMQConnection>();
                    services.AddSingleton<ActivateUserQueueSubscribe>();
                    services.AddHostedService<Worker>();
                });
    }
}
