using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;
using Common.Email;
using System.Net.Mail;

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
            try
            {
                using MailMessage mail = new MailMessage();
                mail.From = new MailAddress("support@encryptionapiservices.com");
                mail.To.Add(message.UserEmail);
                mail.Subject = "Emergency Kit - Encryption API Services";
                mail.Body = "<html>" +
                    "<body>" +
                        "This is your emergency kit for account recovery if you completely forgot your password. Please store it in a safe place. Thanks for registering. <br>" + String.Format("Key: <b>{0}</b>", message.EncappedKey) +
                    "</body>" +
                    "</html>";
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
