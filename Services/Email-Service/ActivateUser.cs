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
            string guid = Guid.NewGuid().ToString();
            RustSHAWrapper shaWrapper = new RustSHAWrapper();
            IntPtr hashedGuidPtr = await shaWrapper.SHA512HashStringAsync(guid);
            string hashedGuid = Marshal.PtrToStringAnsi(hashedGuidPtr);
            RustRSAWrapper rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
            RsaSignResult signtureResult = await rsaWrapper.RsaSignAsync(guid, 4096);
            string signature = Marshal.PtrToStringAnsi(signtureResult.signature);
            string publicKey = Marshal.PtrToStringAnsi(signtureResult.public_key);
            string urlSignature = Base64UrlEncoder.Encode(signature);
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
                await repo.UpdateUsersRsaKeyPairsAndToken(user, publicKey, guid, signature);
                RustRSAWrapper.free_cstring(signtureResult.public_key);
                RustRSAWrapper.free_cstring(signtureResult.signature);
                RustSHAWrapper.free_cstring(hashedGuidPtr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
