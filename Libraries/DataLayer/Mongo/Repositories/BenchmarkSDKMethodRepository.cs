using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class BenchmarkSDKMethodRepository : IBenchmarkSDKMethodRepository
    {
        private readonly IMongoCollection<BenchmarkSDKMethod> _benchmarkSDKMethods;

        public BenchmarkSDKMethodRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._benchmarkSDKMethods = database.GetCollection<BenchmarkSDKMethod>("BenchmarkSDKMethods");
        }

        public async Task InsertSDKMethodBenchmark(BenchmarkSDKMethod method)
        {
            await this._benchmarkSDKMethods.InsertOneAsync(method);
        }

        public async Task<List<BenchmarkSDKMethod>> GetUserBenchmarksDaysAgo(string userId, int daysAgo)
        {
            DateTime timeAgo = DateTime.UtcNow.AddDays(-daysAgo);
            return await this._benchmarkSDKMethods.Find(x => x.CreatedBy == userId && x.MethodStart >= timeAgo && x.MethodEnd >= timeAgo).ToListAsync();
        }
    }
}
