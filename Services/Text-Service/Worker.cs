using DataLayer.Mongo;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Text_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDatabaseSettings _databaseSettings;
        private readonly MongoClient _mongoClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            this._databaseSettings = new DatabaseSettings()
            {
                Connection = Environment.GetEnvironmentVariable("Connection"),
                DatabaseName = Environment.GetEnvironmentVariable("DatabaseName"),
                UserCollectionName = Environment.GetEnvironmentVariable("UserCollectionName")
            };
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
            this._mongoClient = new MongoClient(settings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TwoFactorAuthHotpCode twoFactorHotpCodes = new TwoFactorAuthHotpCode(this._databaseSettings, this._mongoClient);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.WhenAll(
                    twoFactorHotpCodes.GetHotpCodesToSendOut()
                    );
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
