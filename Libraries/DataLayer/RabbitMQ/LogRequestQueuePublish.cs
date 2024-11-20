namespace DataLayer.RabbitMQ
{
    public class LogRequestQueuePublish : RabbitMqPublish
    {
        public LogRequestQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.LogRequest)
        {
        }
    }
}
