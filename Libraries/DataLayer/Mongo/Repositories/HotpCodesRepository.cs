using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
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
        public async Task<long> GetHighestCounter()
        {
            HotpCode hotpCode = await this._hotpCodes.AsQueryable().OrderByDescending(x => x.Counter).FirstOrDefaultAsync();
            if (hotpCode != null)
            {
                return hotpCode.Counter;
            }
            else
            {
                return 0;
            }
        }

        public async Task<HotpCode> GetHotpCodeByIdAndCode(string id)
        {
            return await this._hotpCodes.Find(x => x.UserId == id &&
                                              x.HasBeenVerified == false).SortByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
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
    }
}
