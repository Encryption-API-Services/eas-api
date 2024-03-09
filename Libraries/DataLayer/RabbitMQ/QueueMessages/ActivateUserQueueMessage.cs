namespace DataLayer.RabbitMQ.QueueMessages
{
    public class ActivateUserQueueMessage
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
