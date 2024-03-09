using DataLayer.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Email_Service
{
    public class Worker : BackgroundService
    {
        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            ActivateUserQueueSubscribe activeUserSubscribe,
            ForgotPasswordQueueSubscribe forgotPasswordSubscribe
            )
        {
            
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
