namespace DataLayer.RabbitMQ.QueueMessages
{
    public class EmergencyKitQueueMessage
    {
        public string UserEmail { get; set; }
        public string AesKey { get; set; }
    }
}
