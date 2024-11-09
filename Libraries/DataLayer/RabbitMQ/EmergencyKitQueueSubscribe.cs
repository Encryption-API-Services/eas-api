using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;

namespace DataLayer.RabbitMQ
{
    public class EmergencyKitQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;

        public EmergencyKitQueueSubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.EmergencyKit,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += EmergencyKitMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.EmergencyKit, autoAck: false, consumer: this.Consumer);
        }

        public async void EmergencyKitMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            EmergencyKitSendQueueMessage message = JsonSerializer.Deserialize<EmergencyKitSendQueueMessage>(e.Body.ToArray());
            Console.WriteLine("welcome");
        }
    }
}
