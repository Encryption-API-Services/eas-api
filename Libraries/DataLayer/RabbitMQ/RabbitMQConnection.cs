using RabbitMQ.Client;
using System;

namespace DataLayer.RabbitMQ
{
    public class RabbitMQConnection
    {
        public IConnection Connection { get; set; }
        public RabbitMQConnection()
        {
            this.CreateConnection();
        }
        private void CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.MaxMessageSize = 512 * 1024 * 1024;
            factory.Uri = new Uri(Environment.GetEnvironmentVariable("RabbitMqUrl"));
            this.Connection = factory.CreateConnection();
        }
    }
}
