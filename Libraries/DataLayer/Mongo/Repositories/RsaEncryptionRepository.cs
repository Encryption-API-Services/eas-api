using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class RsaEncryptionRepository : IRsaEncryptionRepository
    {
        private readonly IMongoCollection<RsaEncryption> _rsaEncryptions;
        public RsaEncryptionRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._rsaEncryptions = database.GetCollection<RsaEncryption>("RsaEncryptions");
        }
        public async Task InsertNewEncryption(RsaEncryption newEncryption)
        {
            await this._rsaEncryptions.InsertOneAsync(newEncryption);
        }
        public async Task<RsaEncryption> GetEncryptionByIdAndPublicKey(string userId, string publicKey)
        {
            return await this._rsaEncryptions.Find(x => x.UserId == userId && x.PublicKey == publicKey).FirstOrDefaultAsync();
        }
    }
}