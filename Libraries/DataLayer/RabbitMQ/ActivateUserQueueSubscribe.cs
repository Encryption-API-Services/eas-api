using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Hashers;
using DataLayer.RabbitMQ.QueueMessages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;
using DataLayer.Mongo.Repositories;

namespace DataLayer.RabbitMQ
{
    public class ActivateUserQueueSubscribe
    {
        private IModel Channel { get; set; }
        private EventingBasicConsumer Consumer { get; set; }
        private readonly IUserRepository _userRepository;
        public ActivateUserQueueSubscribe(RabbitMQConnection rabbitMqConnection, IUserRepository userRepository)
        {
            this._userRepository = userRepository;
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
        }

        private async void ActivateUserQueueMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            ActivateUserQueueMessage message = JsonSerializer.Deserialize<ActivateUserQueueMessage>(e.Body.ToArray());
            byte[] guid = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashedGuidBytes = shaWrapper.Hash512(guid);
            RSAWrapper rsaWrapper = new RSAWrapper();
            RsaKeyPairResult keyPair = rsaWrapper.GetKeyPair(4096);
            byte[] signtureResult = rsaWrapper.RsaSignWithKeyBytes(keyPair.PrivateKey, hashedGuidBytes);
            string urlSignature = Base64UrlEncoder.Encode(signtureResult);
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(message.UserEmail);
                    mail.Subject = "Account Activation - Encryption API Services ";
                    mail.Body = "We are excited to have you here </br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/activate?id={0}&token={1}'>Click here to activate</a>", message.UserId, urlSignature);
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

                await this._userRepository.UpdateUsersRsaKeyPairsAndToken(message.UserId, keyPair.PublicKey, Convert.ToBase64String(hashedGuidBytes), urlSignature);
                this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
