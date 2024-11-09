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
        Email2FAHotpCodeQueueSubscribe email2faHotpCodeSubscribe,
        EmergencyKitQueueSubscribe emergencyKeySubscribe,
        EmergencyKitRecoverySubscribe emergencyKitRecoverySubscribe
            ) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
