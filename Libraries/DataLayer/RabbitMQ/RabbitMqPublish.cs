using RabbitMQ.Client;

namespace DataLayer.RabbitMQ
{
    public class RabbitMqPublish
    {
        internal IModel Channel { get; set; }
        private string QueueName { get; set; }
        public RabbitMqPublish(RabbitMQConnection rabbitMqConnection, string queueName)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.QueueName = queueName;
        }

        public void BasicPublish(byte[] body)
        {
            this.Channel.BasicPublish(exchange: string.Empty, routingKey: QueueName, basicProperties: null, body: body);
        }
    }
}
