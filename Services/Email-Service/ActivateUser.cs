using CasDotnetSdk.Asymmetric;
using CasDotnetSdk.Asymmetric.Types;
using CasDotnetSdk.Hashers;
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
    public class ActivateUser
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly MongoClient _mongoClient;
        public ActivateUser(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._databaseSettings = databaseSettings;
            this._mongoClient = mongoClient;
        }
        public async Task GetUsersToActivateSendOutTokens()
        {
            UserRepository repo = new UserRepository(this._databaseSettings, this._mongoClient);
            List<User> usersToSendTokens = await repo.GetUsersMadeWithinLastThirtyMinutes();
            foreach (User user in usersToSendTokens)
            {
                await this.GenerateTokenAndSendOut(user);
            }
        }
        private async Task GenerateTokenAndSendOut(User user)
        {
            UserRepository repo = new UserRepository(this._databaseSettings, this._mongoClient);
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
                    mail.To.Add(user.Email);
                    mail.Subject = "Account Activation - Encryption API Services ";
                    mail.Body = "We are excited to have you here </br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/activate?id={0}&token={1}'>Click here to activate</a>", user.Id, urlSignature);
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        string email = Environment.GetEnvironmentVariable("Email");
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(email, "bzdjmoscoeyzfcsj");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                await repo.UpdateUsersRsaKeyPairsAndToken(user, keyPair.PublicKey, Convert.ToBase64String(hashedGuidBytes), urlSignature);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
