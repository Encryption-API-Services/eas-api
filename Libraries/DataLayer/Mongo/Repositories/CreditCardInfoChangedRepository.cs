using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class CreditCardInfoChangedRepository : ICreditCardInfoChangedRepository
    {
        private readonly IMongoCollection<CreditCardInfoChanged> _collection;
        public CreditCardInfoChangedRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._collection = database.GetCollection<CreditCardInfoChanged>("CreditCardInfoChanged");
        }
        public async Task InsertCreditCardInformationChanged(CreditCardInfoChanged changedInfo)
        {
            await this._collection.InsertOneAsync(changedInfo);
        }
        public async Task<List<CreditCardInfoChanged>> GetUnsentNotifications()
        {
            return await this._collection.Find(x => x.WasSent == false).ToListAsync();
        }
        public async Task UpdateInfoToSent(CreditCardInfoChanged changedInfo)
        {
            var filter = Builders<CreditCardInfoChanged>.Filter.Eq(x => x.Id, changedInfo.Id);
            var definition = Builders<CreditCardInfoChanged>.Update.Set(x => x.WasSent, true);
            await this._collection.UpdateOneAsync(filter, definition);
        }
    }
}