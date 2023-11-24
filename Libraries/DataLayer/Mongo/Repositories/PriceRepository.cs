using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly IMongoCollection<Price> _priceCollection;

        public PriceRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._priceCollection = database.GetCollection<Price>("Prices");
        }

        public async Task<Price> GetPriceByAmount(long amount)
        {
            return await this._priceCollection.Find(x => x.Amount == amount).FirstOrDefaultAsync();
        }

        public async Task<List<Price>> GetPricesByProductId(string stripeProductId)
        {
            return await this._priceCollection.Find(x => x.StripeProductId == stripeProductId).ToListAsync();
        }

        public async Task InsertPrice(Price price)
        {
            await this._priceCollection.InsertOneAsync(price);
        }
    }
}