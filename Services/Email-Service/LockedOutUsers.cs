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
    public class LockedOutUsers
    {
        private readonly IUserRepository _userRepository;

        public LockedOutUsers(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._userRepository = new UserRepository(databaseSettings, mongoClient);
        }
        public async Task GetUsersThatLockedOut()
        {
            List<User> lockedOutUsers = await this._userRepository.GetLockedOutUsers();
            if (lockedOutUsers.Count > 0)
            {
                await this.SendUnlockEmailToUsers(lockedOutUsers);
            }
        }

        private async Task SendUnlockEmailToUsers(List<User> lockedOutUsers)
        {
            foreach (User user in lockedOutUsers)
            {
                try
                {
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    SmtpServer.Port = 587;


                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("support@encryptionapiservices.com");
                        mail.To.Add(user.Email);
                        mail.Subject = "Locked Out User Account - Encryption API Services";
                        mail.Body = "Your account has been locked out due to many failed login attempts.</br>" + String.Format("To unlock your account click <a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/unlock-account?id={0}'>here</a>.", user.Id);
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
                    await this._userRepository.UpdateUserLockedOutToSentOut(user.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
