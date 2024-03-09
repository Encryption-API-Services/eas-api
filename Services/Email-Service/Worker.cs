using DataLayer.RabbitMQ;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Email_Service
{
    public class Worker(
        ActivateUserQueueSubscribe activeUserSubscribe,
        ForgotPasswordQueueSubscribe forgotPasswordSubscribe,
        LockedOutUserQueueSubscribe lockedOutUserSubscribe,
        CreditCardInformationChangedQueueSubscribe ccInfoChangedSubscribe,
        InactiveUser inactiveUsers
            ) : BackgroundService
    {
        private readonly InactiveUser _inactiveUsers = inactiveUsers;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.WhenAll(this._inactiveUsers.GetInactiveUsers());
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
