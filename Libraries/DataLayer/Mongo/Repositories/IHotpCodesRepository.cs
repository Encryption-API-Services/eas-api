using DataLayer.Mongo.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IHotpCodesRepository
    {
        public Task<long> GetHighestCounter();
        public Task InsertHotpCode(HotpCode code);
        public Task<List<HotpCode>> GetAllHotpCodesNotSent();
        public Task UpdateHotpCodeToSent(string id);
        public Task<HotpCode> GetHotpCodeByIdAndCode(string id, string code);
        public Task UpdateHotpToVerified(string id);
    }
}
