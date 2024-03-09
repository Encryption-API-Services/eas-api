using DataLayer.Mongo;
using DataLayer.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Email_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabaseSettings _databaseSettings;
        private readonly MongoClient _mongoClient;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            ActivateUserQueueSubscribe activeUserSubscribe
            )
        {
            _logger = logger;
            _configuration = configuration;
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
                //ForgotPassword forgotPassword = new ForgotPassword(this._databaseSettings, this._mongoClient);
                //LockedOutUsers lockedOutUsers = new LockedOutUsers(this._databaseSettings, this._mongoClient);
                //CCInfoChanged creditCardInfoChanged = new CCInfoChanged(this._databaseSettings, this._mongoClient);
                //InactiveUser inactiveUser = new InactiveUser(this._databaseSettings, this._mongoClient);
                //await Task.WhenAll(
                //    forgotPassword.GetUsersWhoNeedToResetPassword(),
                //    lockedOutUsers.GetUsersThatLockedOut(),
                //    creditCardInfoChanged.GetUsersWhoChangedEmailInfo(),
                //    inactiveUser.GetInactiveUsers()
                //);
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
