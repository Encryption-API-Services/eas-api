using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using static Common.UniqueIdentifiers.Generator;
using System.Net.Mail;
using System.Net;
using Twilio.TwiML.Messaging;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.RabbitMQ
{
    public class Email2FAHotpCodeQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;

        public Email2FAHotpCodeQueueSubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.Email2FAHotpCode,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += Email2FAHotpMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.Email2FAHotpCode, autoAck: false, consumer: this.Consumer);
        }

        private async void Email2FAHotpMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                Email2FAHotpCodeQueueMessage message = JsonSerializer.Deserialize<Email2FAHotpCodeQueueMessage>(e.Body.ToArray());
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(message.UserEmail);
                    mail.Subject = "Email 2FA - Encryption API Services";
                    mail.Body = String.Format("Your login code is: <b>{0}</b>" , message.HotpCode);
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        string email = Environment.GetEnvironmentVariable("Email");
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(email, Environment.GetEnvironmentVariable("EmailPass"));
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
