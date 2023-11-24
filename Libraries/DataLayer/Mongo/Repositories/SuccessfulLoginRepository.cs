using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class SuccessfulLoginRepository : ISuccessfulLoginRepository
    {
        private readonly IMongoCollection<SuccessfulLogin> _successfulLoginRepository;
        public SuccessfulLoginRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._successfulLoginRepository = database.GetCollection<SuccessfulLogin>("SuccessfulLogins");
        }

        public IFindFluent<SuccessfulLogin, SuccessfulLogin> GetAllSuccessfulLoginWithinTimeFrame(string userId, DateTime dateTime)
        {
            return this._successfulLoginRepository.Find(x => x.UserId == userId &&
                                                                        x.CreateTime >= dateTime &&
                                                                        x.WasThisMe == false &&
                                                                        x.HasBeenChecked == false).SortByDescending(x => x.CreateTime);
        }

        public async Task InsertSuccessfulLogin(SuccessfulLogin login)
        {
            await this._successfulLoginRepository.InsertOneAsync(login);
        }

        public async Task UpdateSuccessfulLoginWasMe(string loginId, bool wasThisMe)
        {
            var filter = Builders<SuccessfulLogin>.Filter.Eq(x => x.Id, loginId);
            var update = Builders<SuccessfulLogin>.Update.Set(x => x.WasThisMe, wasThisMe)
                                                         .Set(x => x.HasBeenChecked, true);
            await this._successfulLoginRepository.UpdateOneAsync(filter, update);
        }
        public async Task<long> GetLoginsCountAfterDate(DateTime timePeriod, string userId)
        {
            return await this._successfulLoginRepository.Find(x => x.CreateTime >= timePeriod && x.UserId == userId).CountDocumentsAsync();
        }
    }
}
