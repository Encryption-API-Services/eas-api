using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.RabbitMQ
{
    public class EmergencyKitQueuePublish : RabbitMqPublish 
    {
        public EmergencyKitQueuePublish(RabbitMQConnection rabbitMqConnection) : base(rabbitMqConnection, RabbitMqConstants.Queues.EmergencyKit)
        {
        }
    }
}
