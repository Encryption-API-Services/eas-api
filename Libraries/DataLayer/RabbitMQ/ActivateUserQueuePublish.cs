using RabbitMQ.Client;

namespace DataLayer.RabbitMQ
{
    public class ActivateUserQueuePublish : RabbitMqPublish
    { 
        public ActivateUserQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.ActivateUser)
        {
            
        }
    }
}
