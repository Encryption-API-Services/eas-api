using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Hashers;
using Common.UniqueIdentifiers;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ.QueueMessages;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using static Common.UniqueIdentifiers.Generator;

namespace DataLayer.RabbitMQ
{
    public class ActivateUserQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        private readonly IUserRepository _userRepository;
        public ActivateUserQueueSubscribe(RabbitMQConnection rabbitMqConnection, IUserRepository userRepository)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.ActivateUser,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += ActivateUserQueueMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.ActivateUser, autoAck: false, consumer: this.Consumer);
            this._userRepository = userRepository;
        }

        private async void ActivateUserQueueMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            ActivateUserQueueMessage message = JsonSerializer.Deserialize<ActivateUserQueueMessage>(e.Body.ToArray());
            EmailToken emailToken = new Generator().GenerateEmailToken();
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(message.UserEmail);
                    mail.Subject = "Account Activation - Encryption API Services ";
                    mail.Body = "We are excited to have you here </br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/activate?id={0}&token={1}'>Click here to activate</a>", message.UserId, emailToken.UrlSignature);
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

                await this._userRepository.UpdateUsersRsaKeyPairsAndToken(message.UserId, emailToken.Base64PublicKey, emailToken.Base64HashedToken, emailToken.UrlSignature);
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
