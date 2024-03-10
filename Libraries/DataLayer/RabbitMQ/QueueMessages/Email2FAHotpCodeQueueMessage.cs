namespace DataLayer.RabbitMQ.QueueMessages
{
    public class Email2FAHotpCodeQueueMessage
    {
        public string UserEmail { get; set; }
        public string HotpCode { get; set; }
    }
}
