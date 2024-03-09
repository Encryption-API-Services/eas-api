namespace DataLayer.RabbitMQ
{
    public class LockedOutUserQueuePublish : RabbitMqPublish
    {
        public LockedOutUserQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.LockedOutUsers)
        {
        }
    }
}
