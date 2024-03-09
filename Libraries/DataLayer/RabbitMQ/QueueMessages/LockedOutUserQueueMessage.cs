namespace DataLayer.RabbitMQ.QueueMessages
{
    public class LockedOutUserQueueMessage
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
