﻿using Common.Email;
using DataLayer.RabbitMQ.QueueMessages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Text.Json;

namespace DataLayer.RabbitMQ
{
    public class CreditCardInformationChangedQueueSubscribe
    {
        private readonly IModel Channel;
        private readonly EventingBasicConsumer Consumer;
        public CreditCardInformationChangedQueueSubscribe(RabbitMQConnection rabbitMqConnection)
        {
            this.Channel = rabbitMqConnection.Connection.CreateModel();
            this.Channel.QueueDeclare(
                queue: RabbitMqConstants.Queues.CCInformationChanged,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            this.Consumer = new EventingBasicConsumer(this.Channel);
            this.Consumer.Received += CreditCardInformationChangedMessageReceived;
            this.Channel.BasicConsume(queue: RabbitMqConstants.Queues.CCInformationChanged, autoAck: false, consumer: this.Consumer);
        }

        private async void CreditCardInformationChangedMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            CreditCardInformationChangedQueueMessage message = JsonSerializer.Deserialize<CreditCardInformationChangedQueueMessage>(e.Body.ToArray());
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            using MailMessage mail = new MailMessage();
            mail.From = new MailAddress("support@encryptionapiservices.com");
            mail.To.Add(message.UserEmail);
            mail.Subject = "Credit Card Changed - Encryption API Services";
            mail.Body = "We noticed that you changed your credit card information recently. If this wasn't you we recommend changing your password " + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/forgot-password'>here</a>");
            mail.IsBodyHtml = true;
            SmtpClientSender.SendMailMessage(mail);
            this.Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }
    }
}