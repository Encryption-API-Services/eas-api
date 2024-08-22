namespace DataLayer.RabbitMQ
{
    public class EmergencyKitRecoveredPublish : RabbitMqPublish
    {
        public EmergencyKitRecoveredPublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.EmergencyKitRecovered)
        {
        }
    }
}
