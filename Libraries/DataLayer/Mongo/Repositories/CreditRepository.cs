using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class CreditRepository : ICreditRepository
    {
        private readonly IMongoCollection<ValidatedCreditCard> _validatedCreditCards;
        public CreditRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._validatedCreditCards = database.GetCollection<ValidatedCreditCard>("ValidateCreditCards");
        }

        public async Task AddValidatedCreditInformation(ValidatedCreditCard card)
        {
            await this._validatedCreditCards.InsertOneAsync(card);
        }
    }
}
