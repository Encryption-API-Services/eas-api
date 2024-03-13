using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Hashers;
using Common.Email;
using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Email_Service
{
    public class InactiveUser
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly IUserRepository _userRepository;
        private readonly ISuccessfulLoginRepository _successfulLoginRepository;

        public InactiveUser(IDatabaseSettings databaseSettings, IMongoClient mongoClient)
        {
            this._databaseSettings = databaseSettings;
            this._userRepository = new UserRepository(this._databaseSettings, mongoClient);
            this._successfulLoginRepository = new SuccessfulLoginRepository(this._databaseSettings, mongoClient);
        }

        public async Task GetInactiveUsers()
        {
            List<User> activeUsers = await this._userRepository.GetActiveUsers();
            foreach (User user in activeUsers)
            {
                // Check if the user has any logins within the last 90 days
                long successfulLogins = await this._successfulLoginRepository.GetLoginsCountAfterDate(DateTime.UtcNow.AddDays(-90), user.Id);
                if (successfulLogins == 0 && user.InactiveEmail.Sent == false)
                {
                    await this.SendUserEmail(user);
                }
            }
        }

        private async Task SendUserEmail(User user)
        {
            string guid = Guid.NewGuid().ToString();
            byte[] guidBytes = Encoding.UTF8.GetBytes(guid);
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashedGuid = shaWrapper.Hash512(guidBytes);
            RSAWrapper rsaWrapper = new RSAWrapper();
            RsaKeyPairResult keyPair = rsaWrapper.GetKeyPair(4096);
            byte[] signature = rsaWrapper.RsaSignWithKeyBytes(keyPair.PrivateKey, hashedGuid);
            string urlSignature = Base64UrlEncoder.Encode(signature);
            using MailMessage mail = new MailMessage();
            mail.From = new MailAddress("support@encryptionapiservices.com");
            mail.To.Add(user.Email);
            mail.Subject = "Inactive User - Encryption API Services";
            mail.Body = "We noticed you haven't logged in for 3 months. If you want to delete your user account you can do so by clicking </br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/inactive-user?id={0}&token={1}'>here</a>.", user.Id, urlSignature);
            mail.IsBodyHtml = true;
            SmtpClientSender.SendMailMessage(mail);
            await this._userRepository.UpdateInactiveEmailSent(user.Id, Convert.ToBase64String(hashedGuid), keyPair.PublicKey);
        }
    }
}