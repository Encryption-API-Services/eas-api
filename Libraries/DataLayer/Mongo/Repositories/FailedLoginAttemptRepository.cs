using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class FailedLoginAttemptRepository : IFailedLoginAttemptRepository
    {
        private readonly IMongoCollection<FailedLoginAttempt> _failedLoginAttempts;
        public FailedLoginAttemptRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._failedLoginAttempts = database.GetCollection<FailedLoginAttempt>("FailedLoginAttempts");
        }

        public async Task<List<FailedLoginAttempt>> GetFailedLoginAttemptsLastTweleveHours(string userId)
        {
            return await this._failedLoginAttempts.FindAsync(x => x.UserAccount == userId && x.CreateDate >= DateTime.UtcNow.AddHours(-12)).GetAwaiter().GetResult().ToListAsync();
        }

        public async Task InsertFailedLoginAttempt(FailedLoginAttempt loginAttempt)
        {
            await this._failedLoginAttempts.InsertOneAsync(loginAttempt);
        }
    }
}