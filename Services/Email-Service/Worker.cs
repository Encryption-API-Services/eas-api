using DataLayer.Mongo;
using DataLayer.RabbitMQ;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Email_Service
{
    public class Worker : BackgroundService
    {
        public Worker(
            IDatabaseSettings databaseSettings,
            IMongoClient mongoClient,
            ActivateUserQueueSubscribe activeUserSubscribe,
            ForgotPasswordQueueSubscribe forgotPasswordSubscribe,
            LockedOutUserQueueSubscribe lockedOutUserSubscribe,
            CreditCardInformationChangedQueueSubscribe ccInfoChangedSubscribe
            )
        { }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //CCInfoChanged creditCardInfoChanged = new CCInfoChanged(this.databaseSettings, this.mongoClient);
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
