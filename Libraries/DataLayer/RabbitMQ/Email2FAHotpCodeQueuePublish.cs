namespace DataLayer.RabbitMQ
{
    public class Email2FAHotpCodeQueuePublish : RabbitMqPublish
    {
        public Email2FAHotpCodeQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.Email2FAHotpCode)
        {
        }
    }
}
