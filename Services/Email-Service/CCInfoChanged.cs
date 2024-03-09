using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email_Service
{
    public class CCInfoChanged
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly ICreditCardInfoChangedRepository _creditCardInfoChangedRepository;
        public CCInfoChanged(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._databaseSettings = databaseSettings;
            this._creditCardInfoChangedRepository = new CreditCardInfoChangedRepository(this._databaseSettings, mongoClient);
        }
        public async Task GetUsersWhoChangedEmailInfo()
        {
            List<CreditCardInfoChanged> emailsToSend = await this._creditCardInfoChangedRepository.GetUnsentNotifications();
            for (int i = 0; i < emailsToSend.Count; i++)
            {
                await this.SendEmail(emailsToSend[i]);
            }
        }
        private async Task SendEmail(CreditCardInfoChanged info)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("support@encryptionapiservices.com");
                mail.To.Add(info.Email);
                mail.Subject = "Credit Card Changed - Encryption API Services";
                mail.Body = "We noticed that you changed your credit card information recently. If this wasn't you we recommend changing your password " + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/forgot-password'>here</a>");
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    string email = Environment.GetEnvironmentVariable("Email");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(email, Environment.GetEnvironmentVariable("EmailPass"));
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                await this._creditCardInfoChangedRepository.UpdateInfoToSent(info);
            }
        }
    }
}