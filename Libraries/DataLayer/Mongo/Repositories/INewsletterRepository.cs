using DataLayer.Mongo.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface INewsletterRepository
    {
        public Task<Newsletter> GetSubscriptionByEmail(string email);
        public Task AddEmailToNewsletter(Newsletter newsletter);
        public Task<List<Newsletter>> GetAllNewsletters();
    }
}
