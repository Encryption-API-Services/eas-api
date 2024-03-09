using DataLayer.RabbitMQ.QueueMessages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;
using System.Threading.Channels;

namespace DataLayer.RabbitMQ
{
    public class ActivateUserQueueSubscribe
    {
        private IModel Channel { get; set; }
        private EventingBasicConsumer Consumer { get; set; }
        public ActivateUserQueueSubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.ActiveUserQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += ActivateUserQueueMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.ActiveUserQueue, autoAck: false, consumer: this.Consumer);
        }

        private void ActivateUserQueueMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            ActivateUserQueueMessage message = JsonSerializer.Deserialize<ActivateUserQueueMessage>(body);
            var testing = "1234";
        }
    }
}
