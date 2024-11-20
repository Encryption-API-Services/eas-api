using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;

namespace DataLayer.RabbitMQ
{
    public class LogRequestQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        private readonly ILogRequestRepository logRequestRepository;
        public LogRequestQueueSubscribe(
            RabbitMQConnection rabbitMqConnection,
            ILogRequestRepository logRequestRepository
            )
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.LogRequest,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += LogRequest;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.LogRequest, autoAck: false, consumer: this.Consumer);
            this.logRequestRepository = logRequestRepository;
        }

        private async void LogRequest(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                LogRequest request = JsonSerializer.Deserialize<LogRequest>(e.Body.ToArray());
                await this.logRequestRepository.InsertRequest(request);
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
