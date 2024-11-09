namespace DataLayer.RabbitMQ
{
    public class EmergencyKitRecoveryPublish : RabbitMqPublish
    {
        public EmergencyKitRecoveryPublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.EmergencyKitRecovery)
        {
        }
    }
}
