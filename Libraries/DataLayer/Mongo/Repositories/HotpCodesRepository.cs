using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class HotpCodesRepository : IHotpCodesRepository
    {
        private readonly IMongoCollection<HotpCode> _hotpCodes;

        public HotpCodesRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._hotpCodes = database.GetCollection<HotpCode>("HotpCodes");
        }
        public async Task<List<HotpCode>> GetAllHotpCodesNotSent()
        {
            return await this._hotpCodes.AsQueryable().Where(x => x.HasBeenSent == false && x.HasBeenVerified == false).ToListAsync();
        }

        public async Task<long> GetHighestCounter()
        {
            HotpCode hotpCode = await this._hotpCodes.AsQueryable().OrderByDescending(x => x.Counter).FirstAsync();
            return hotpCode.Counter;
        }

        public async Task<HotpCode> GetHotpCodeByIdAndCode(string id, string code)
        {
            return await this._hotpCodes.Find(x => x.UserId == id &&
                                              x.Hotp == code &&
                                              x.HasBeenSent == true &&
                                              x.HasBeenVerified == false).FirstOrDefaultAsync();
        }

        public async Task InsertHotpCode(HotpCode code)
        {
            await this._hotpCodes.InsertOneAsync(code);
        }

        public async Task UpdateHotpToVerified(string id)
        {
            var filter = Builders<HotpCode>.Filter.Eq(x => x.Id, id);
            var update = Builders<HotpCode>.Update.Set(x => x.HasBeenVerified, true);
            await this._hotpCodes.UpdateOneAsync(filter, update);
        }

        public async Task UpdateHotpCodeToSent(string id)
        {
            var filter = Builders<HotpCode>.Filter.Eq(x => x.Id, id);
            var update = Builders<HotpCode>.Update.Set(x => x.HasBeenSent, true);
            await this._hotpCodes.UpdateOneAsync(filter, update);
        }
    }
}
