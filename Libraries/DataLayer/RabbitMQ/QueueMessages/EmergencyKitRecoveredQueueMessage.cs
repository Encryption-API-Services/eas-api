namespace DataLayer.RabbitMQ.QueueMessages
{
    public class EmergencyKitRecoveredQueueMessage
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
