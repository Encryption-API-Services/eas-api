namespace DataLayer.RabbitMQ
{
    public class CreditCardInformationChangedQueuePublish : RabbitMqPublish
    {
        public CreditCardInformationChangedQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.CCInformationChanged)
        {
        }
    }
}
