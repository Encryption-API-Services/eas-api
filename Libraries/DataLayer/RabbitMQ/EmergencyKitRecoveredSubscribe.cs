using Common.Email;
using DataLayer.RabbitMQ.QueueMessages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Text.Json;

namespace DataLayer.RabbitMQ
{
    public class EmergencyKitRecoveredSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;

        public EmergencyKitRecoveredSubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.EmergencyKitRecovered,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += EmergencyKitRecoveredSubscribeReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.EmergencyKitRecovered, autoAck: false, consumer: this.Consumer);
        }

        public async void EmergencyKitRecoveredSubscribeReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                EmergencyKitRecoveredQueueMessage message = JsonSerializer.Deserialize<EmergencyKitRecoveredQueueMessage>(e.Body.ToArray());
                using MailMessage mail = new MailMessage();
                mail.From = new MailAddress("support@cryptographicapiservices.com");
                mail.To.Add(message.Email);
                mail.Subject = "Account Reactivation Reset - Cryptographic API Services";
                mail.Body = "We have successfully reset your account and provided you with a new password to login with. </br>" +
                            String.Format("Password: <b>{0}</b>", message.NewPassword);
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
