namespace DataLayer.RabbitMQ.QueueMessages
{
    public class EmergencyKitRecoveryQueueMessage
    {
        public string UserEmail { get; set; }
        public string NewPassword { get; set; }
    }
}
