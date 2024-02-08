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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CasDotnetSdk.Asymmetric.RSAWrapper;

namespace Email_Service
{
    public class ForgotPassword
    {
        private readonly IUserRepository _userRepository;

        public ForgotPassword(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._userRepository = new UserRepository(databaseSettings, mongoClient);
        }
        public async Task GetUsersWhoNeedToResetPassword()
        {
            List<User> users = await this._userRepository.GetUsersWhoForgotPassword();
            if (users.Count > 0)
            {
                await this.SendOutForgotEmails(users);
            }
        }

        private async Task SendOutForgotEmails(List<User> users)
        {
            foreach (User user in users)
            {
                string guid = Guid.NewGuid().ToString();
                byte[] guidBytes = Encoding.UTF8.GetBytes(guid);
                SHAWrapper shaWrapper = new SHAWrapper();
                byte[] hashedGuid =  shaWrapper.Hash512(guidBytes);
                RSAWrapper rsaWrapper = new RSAWrapper();
                RsaKeyPairResult keyPair = rsaWrapper.GetKeyPair(4096);
                byte[] signature = rsaWrapper.RsaSignWithKeyBytes(keyPair.PrivateKey, hashedGuid);
                string urlSignature = Base64UrlEncoder.Encode(signature);
                try
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("support@encryptionapiservices.com");
                        mail.To.Add(user.Email);
                        mail.Subject = "Forgot Password - Encryption API Services";
                        mail.Body = "If you did not ask to reset this password please delete this email.</br>" + String.Format("<a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/forgot-password/reset?id={0}&token={1}'>Click here to reset your password.</a>", user.Id, urlSignature);
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
                    await this._userRepository.UpdateUsersForgotPasswordToReset(user.Id, Convert.ToBase64String(hashedGuid), keyPair.PublicKey, urlSignature);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
