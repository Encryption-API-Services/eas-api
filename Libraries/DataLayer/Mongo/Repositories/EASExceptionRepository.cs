using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class EASExceptionRepository : IEASExceptionRepository
    {
        private readonly IMongoCollection<EASException> _exceptions;

        public EASExceptionRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._exceptions = database.GetCollection<EASException>("Exceptions");
        }
        public async Task InsertException(string exceptionMessage, string methodBase)
        {
            await this._exceptions.InsertOneAsync(new EASException()
            {
                ExceptionBody = exceptionMessage,
                CreateDate = DateTime.UtcNow,
                Method = methodBase
            });
        }
    }
}