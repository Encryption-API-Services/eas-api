using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;
using Common.Email;
using static Common.UniqueIdentifiers.Generator;
using System.Net.Mail;
using System;

namespace DataLayer.RabbitMQ
{
    public class EmergencyKitRecoverySubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        public EmergencyKitRecoverySubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.EmergencyKitRecovery,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += EmergencyKitRecoveryReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.EmergencyKitRecovery, autoAck: false, consumer: this.Consumer);
        }

        public async void EmergencyKitRecoveryReceived(object? sender, BasicDeliverEventArgs e)
        {
            EmergencyKitRecoveryQueueMessage message = JsonSerializer.Deserialize<EmergencyKitRecoveryQueueMessage>(e.Body.ToArray());
            try
            {
                using MailMessage mail = new MailMessage();
                mail.From = new MailAddress("support@encryptionapiservices.com");
                mail.To.Add(message.UserEmail);
                mail.Subject = "Emergency Kit Recovery - Encryption API Services";
                mail.Body = String.Format("Your account has been unlocked and the new password is: <b>{0}</b>", message.NewPassword);
                mail.IsBodyHtml = true;
                SmtpClientSender.SendMailMessage(mail);
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
