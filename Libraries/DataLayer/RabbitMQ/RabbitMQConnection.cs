using RabbitMQ.Client;

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
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = ConnectionFactory.DefaultVHost;
            factory.HostName = "rabbit-mq";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;
            factory.MaxMessageSize = 512 * 1024 * 1024;
            this.Connection = factory.CreateConnection();
        }
    }
}
