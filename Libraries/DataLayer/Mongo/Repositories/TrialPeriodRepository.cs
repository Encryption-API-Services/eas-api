using Models.TrialPeriod;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class TrialPeriodRepository : ITrialPeriodRepository
    {

        private readonly IMongoCollection<TrialPeriodRequest> _trialPeriodCollection;
        public TrialPeriodRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._trialPeriodCollection = database.GetCollection<TrialPeriodRequest>("TrialPeriodRequests");
        }
        public async Task Insert(TrialPeriodRequest trialPeriodRequest)
        {
            await this._trialPeriodCollection.InsertOneAsync(trialPeriodRequest);
        }

        public async Task<long> GetTrialPeriodRequestsCount(string userId)
        {
            return await this._trialPeriodCollection.Find(x => x.UserId == userId).CountDocumentsAsync();
        }
    }
}
