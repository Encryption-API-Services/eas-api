namespace DataLayer.RabbitMQ.QueueMessages
{
    public class EmergencyKitSendQueueMessage
    {
        public string UserEmail { get; set; }
        public string EncappedKey { get; set; }
        public string CipherText { get; set; }
    }
}
