using DataLayer.Mongo;
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

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
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
                this._logger.LogInformation("EAS-Email-Service running at: {time}", DateTimeOffset.Now);
                ActivateUser activeUsers = new ActivateUser(this._databaseSettings, this._mongoClient);
                ForgotPassword forgotPassword = new ForgotPassword(this._databaseSettings, this._mongoClient);
                LockedOutUsers lockedOutUsers = new LockedOutUsers(this._databaseSettings, this._mongoClient);
                CCInfoChanged creditCardInfoChanged = new CCInfoChanged(this._databaseSettings, this._mongoClient);
                BlogPostNewsletter blogPostNewsletter = new BlogPostNewsletter(this._databaseSettings, this._mongoClient);
                InactiveUser inactiveUser = new InactiveUser(this._databaseSettings, this._mongoClient);
                await Task.WhenAll(
                    activeUsers.GetUsersToActivateSendOutTokens(),
                    forgotPassword.GetUsersWhoNeedToResetPassword(),
                    lockedOutUsers.GetUsersThatLockedOut(),
                    creditCardInfoChanged.GetUsersWhoChangedEmailInfo(),
                    blogPostNewsletter.SendNewslettersForBlogPosts(),
                    inactiveUser.GetInactiveUsers()
                );
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
