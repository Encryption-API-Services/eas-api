namespace DataLayer.RabbitMQ
{
    public class ForgotPasswordQueuePublish : RabbitMqPublish
    {
        public ForgotPasswordQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.ForgotPassword)
        {
        }
    }
}
