using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class CASExceptionRepository : ICASExceptionRepository
    {
        private readonly IMongoCollection<CASException> _exceptions;

        public CASExceptionRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._exceptions = database.GetCollection<CASException>("Exceptions");
        }
        public async Task InsertException(string exceptionMessage, string methodBase)
        {
            await this._exceptions.InsertOneAsync(new CASException()
            {
                ExceptionBody = exceptionMessage,
                CreateDate = DateTime.UtcNow,
                Method = methodBase
            });
        }
    }
}