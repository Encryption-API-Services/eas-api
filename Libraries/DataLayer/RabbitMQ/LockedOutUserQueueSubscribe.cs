using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Net.Mail;
using System.Net;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;
using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Hashers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataLayer.Mongo.Repositories;
using static Common.UniqueIdentifiers.Generator;
using Common.UniqueIdentifiers;

namespace DataLayer.RabbitMQ
{
    public class LockedOutUserQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        private readonly IUserRepository userRepo;
        public LockedOutUserQueueSubscribe(
            RabbitMQConnection rabbitMqConnection,
            IUserRepository userRepository
            )
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.LockedOutUsers,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += LockedOutUserMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.LockedOutUsers, autoAck: false, consumer: this.Consumer);
            this.userRepo = userRepository;
        }

        private async void LockedOutUserMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                LockedOutUserQueueMessage message = JsonSerializer.Deserialize<LockedOutUserQueueMessage>(e.Body.ToArray());
                EmailToken emailToken = new Generator().GenerateEmailToken();

                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(message.UserEmail);
                    mail.Subject = "Locked Out User Account - Encryption API Services";
                    mail.Body = "Your account has been locked out due to many failed login attempts.</br>" + String.Format("To unlock your account click <a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/unlock-account?id={0}&token={1}'>here</a>.", message.UserId, emailToken.UrlSignature);
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
                await this.userRepo.UpdateLockedOutUsersToken(message.UserId, emailToken.Base64HashedToken, emailToken.Base64PublicKey);
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
