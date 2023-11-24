using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class HashedPasswordRepository : IHashedPasswordRepository
    {
        private readonly IMongoCollection<HashedPassword> _hashedPasswords;

        public HashedPasswordRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._hashedPasswords = database.GetCollection<HashedPassword>("HashedPasswords");
        }

        public async Task InsertOneHasedPassword(HashedPassword password)
        {
            await this._hashedPasswords.InsertOneAsync(password);
        }
        public async Task<HashedPassword> GetOneHashedPassword(string id)
        {
            return await this._hashedPasswords.FindAsync(x => x.Id == id).GetAwaiter().GetResult().FirstOrDefaultAsync();
        }
    }
}
