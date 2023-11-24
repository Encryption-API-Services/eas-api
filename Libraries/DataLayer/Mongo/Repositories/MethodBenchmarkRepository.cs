using Common;
using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class MethodBenchmarkRepository : IMethodBenchmarkRepository
    {
        private readonly IMongoCollection<BenchmarkMethod> _benchmarkCollection;
        private readonly IDatabaseSettings _databaseSettings;
        public MethodBenchmarkRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._benchmarkCollection = database.GetCollection<BenchmarkMethod>("BenchmarkMethod");
        }

        public async Task<List<BenchmarkMethod>> GetAmountByEndTimeDescending(int amountToTake)
        {
            return await this._benchmarkCollection.AsQueryable().OrderByDescending(x => x.Details.EndTime).Take(amountToTake).ToListAsync();
        }

        public async Task InsertBenchmark(BenchmarkMethodLogger method)
        {
            BenchmarkMethod newMethod = new BenchmarkMethod();
            newMethod.Details = method;
            await this._benchmarkCollection.InsertOneAsync(newMethod);
        }

        public async Task InsertBenchmarks(List<BenchmarkMethod> methods)
        {
            await this._benchmarkCollection.InsertManyAsync(methods);
        }
    }
}
