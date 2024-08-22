namespace DataLayer.RabbitMQ
{
    public class EmergencyKitQueuePublish : RabbitMqPublish
    {
        public EmergencyKitQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.EmergencyKit)
        {
        }
    }
}
