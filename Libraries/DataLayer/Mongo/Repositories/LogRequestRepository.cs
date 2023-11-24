using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class LogRequestRepository : ILogRequestRepository
    {
        private readonly IMongoCollection<LogRequest> _logRequestCollection;
        public LogRequestRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._logRequestCollection = database.GetCollection<LogRequest>("LogRequest");
        }
        public async Task InsertRequest(LogRequest request)
        {
            await this._logRequestCollection.InsertOneAsync(request);
        }
        public async Task InsertRequests(List<LogRequest> requests)
        {
            await this._logRequestCollection.InsertManyAsync(requests);
        }
    }
}
