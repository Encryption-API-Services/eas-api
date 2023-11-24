using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Encryption;
using Encryption.Compression;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Encryption.RustRSAWrapper;

namespace Email_Service
{
    public class InactiveUser
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly IUserRepository _userRepository;
        private readonly ISuccessfulLoginRepository _successfulLoginRepository;

        public InactiveUser(IDatabaseSettings databaseSettings, MongoClient mongoClient)
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
            RustSHAWrapper shaWrapper = new RustSHAWrapper();
            IntPtr hashedGuidPtr = await shaWrapper.SHA512HashStringAsync(guid);
            string hashedGuid = Marshal.PtrToStringAnsi(hashedGuidPtr);
            RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
            RsaSignResult signtureResult = await rsaWrapper.RsaSignAsync(guid, 4096);
            string signature = Marshal.PtrToStringAnsi(signtureResult.signature);
            string publicKey = Marshal.PtrToStringAnsi(signtureResult.public_key);
            string urlSignature = Base64UrlEncoder.Encode(signature);
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("support@encryptionapiservices.com");
                mail.To.Add(user.Email);
                mail.Subject = "Inactive User - Encryption API Services";
                mail.Body = "We noticed you haven't logged in for 3 months. If you want to delete your user account you can do so by clicking </br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/inactive-user?id={0}&token={1}'>here</a>.", user.Id, urlSignature);
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    string email = Environment.GetEnvironmentVariable("Email");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(email, "pwqnyquwgjsxhosv");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            await this._userRepository.UpdateInactiveEmailSent(user.Id, guid, publicKey);
            RustRSAWrapper.free_cstring(signtureResult.public_key);
            RustRSAWrapper.free_cstring(signtureResult.signature);
            RustSHAWrapper.free_cstring(hashedGuidPtr);
        }
    }
}