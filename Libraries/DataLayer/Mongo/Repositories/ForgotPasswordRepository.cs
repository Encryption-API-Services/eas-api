using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly IMongoCollection<ForgotPasswordAttempt> _forgotPasswordAttempts;
        public ForgotPasswordRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._forgotPasswordAttempts = database.GetCollection<ForgotPasswordAttempt>("ForgotPasswordAttempts");
        }
        public async Task<List<string>> GetLastFivePassword(string userId)
        {
            return await this._forgotPasswordAttempts.AsQueryable()
                                                     .Where(x => x.UserId == userId)
                                                     .OrderByDescending(x => x.CreateTime)
                                                     .Select(x => x.Password)
                                                     .Take(5)
                                                     .ToListAsync();
        }

        public async Task InsertForgotPasswordAttempt(string userId, string password)
        {
            await this._forgotPasswordAttempts.InsertOneAsync(new ForgotPasswordAttempt()
            {
                UserId = userId,
                Password = password,
                CreateTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            });
        }
    }
}
