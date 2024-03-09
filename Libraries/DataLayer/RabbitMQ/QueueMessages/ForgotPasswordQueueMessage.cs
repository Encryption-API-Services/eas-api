namespace DataLayer.RabbitMQ.QueueMessages
{
    public class ForgotPasswordQueueMessage
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
