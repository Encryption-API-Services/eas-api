using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly IMongoCollection<Newsletter> _newsletter;

        public NewsletterRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._newsletter = database.GetCollection<Newsletter>("Newsletter");
        }

        public async Task AddEmailToNewsletter(Newsletter newsletter)
        {
            await this._newsletter.InsertOneAsync(newsletter);
        }

        public async Task<List<Newsletter>> GetAllNewsletters()
        {
            return await this._newsletter.Find(x => x.Email != null).ToListAsync();
        }

        public async Task<Newsletter> GetSubscriptionByEmail(string email)
        {
            return await this._newsletter.Find(x => x.Email == email).FirstOrDefaultAsync();
        }
    }
}