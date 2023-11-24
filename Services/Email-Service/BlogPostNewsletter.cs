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
    public class BlogPostNewsletter
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly MongoClient _mongoClient;
        public BlogPostNewsletter(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._databaseSettings = databaseSettings;
            this._mongoClient = mongoClient;
        }
        public async Task SendNewslettersForBlogPosts()
        {
            BlogPostRepository blogPostRepo = new BlogPostRepository(this._databaseSettings, this._mongoClient);
            NewsletterRepository newsLetterRepo = new NewsletterRepository(this._databaseSettings, this._mongoClient);
            // Get all blog posts that haven't been sent in the news letter.
            List<BlogPost> blogPostsNotSend = await blogPostRepo.GetBlogPostsNotSentInNewsletter();
            if (blogPostsNotSend.Count > 0)
            {
                List<Newsletter> subscribers = await newsLetterRepo.GetAllNewsletters();
                foreach (BlogPost post in blogPostsNotSend)
                {
                    SendOutEmailsToSubscriber(subscribers, post);
                    await blogPostRepo.UpdateBlogPostSentInNewsLetter(post.Id);
                }
            }
        }
        public void SendOutEmailsToSubscriber(List<Newsletter> subscribers, BlogPost post)
        {
            foreach (Newsletter newsletter in subscribers)
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("support@encryptionapiservices.com");
                    mail.To.Add(newsletter.Email);
                    mail.Subject = "New Blog Post - Encryption API Services";
                    mail.Body = String.Format("Encryption API Services has a new blog post titled {0}. </br>", post.BlogTitle) +
                        String.Format("You can view it <a href='" + Environment.GetEnvironmentVariable("Domain") + "/#/blog/blog-post/" + "{0}' " + "target='_blank'>here</a>.", post.BlogTitle.Replace(" ", "-"));
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

            }
        }
    }
}