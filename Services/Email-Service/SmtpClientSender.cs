using System;
using System.Net;
using System.Net.Mail;

namespace Email_Service
{
    public static class SmtpClientSender
    {
        public static void SendMailMessage(MailMessage mail)
        {
            using SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            string email = Environment.GetEnvironmentVariable("Email");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(email, Environment.GetEnvironmentVariable("EmailPass"));
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
