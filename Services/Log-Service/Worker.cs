using DataLayer.RabbitMQ;

namespace Log_Service
{
    public class Worker : BackgroundService
    {


        public Worker(LogRequestQueueSubscribe logRequestQueueSubscribe)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
