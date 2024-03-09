using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using DataLayer.RabbitMQ.QueueMessages;
using System.Text.Json;
using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Hashers;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;
using System.Text;
using System;
using DataLayer.Mongo.Repositories;

namespace DataLayer.RabbitMQ
{
    public class ForgotPasswordQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        private readonly IUserRepository _userRepository;
        public ForgotPasswordQueueSubscribe(RabbitMQConnection rabbitMqConnection, IUserRepository userRepository)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.ForgotPassword,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += ForgotPasswordQueueMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.ForgotPassword, autoAck: false, consumer: this.Consumer);
            this._userRepository = userRepository;
        }

        private async void ForgotPasswordQueueMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            ForgotPasswordQueueMessage message = JsonSerializer.Deserialize<ForgotPasswordQueueMessage>(e.Body.ToArray());
            string guid = Guid.NewGuid().ToString();
            byte[] guidBytes = Encoding.UTF8.GetBytes(guid);
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashedGuid = shaWrapper.Hash512(guidBytes);
            RSAWrapper rsaWrapper = new RSAWrapper();
            RsaKeyPairResult keyPair = rsaWrapper.GetKeyPair(4096);
            byte[] signature = rsaWrapper.RsaSignWithKeyBytes(keyPair.PrivateKey, hashedGuid);
            string urlSignature = Base64UrlEncoder.Encode(signature);
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(message.UserEmail);
                    mail.Subject = "Forgot Password - Encryption API Services";
                    mail.Body = "If you did not ask to reset this password please delete this email.</br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/forgot-password/reset?id={0}&token={1}'>Click here to reset your password.</a>", message.UserId, urlSignature);
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
                await this._userRepository.UpdateUsersForgotPasswordToReset(message.UserId, Convert.ToBase64String(hashedGuid), keyPair.PublicKey, urlSignature);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
