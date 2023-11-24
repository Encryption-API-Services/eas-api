using DataLayer.Mongo.Entities;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IRsaEncryptionRepository
    {
        Task InsertNewEncryption(RsaEncryption newEncryption);
        Task<RsaEncryption> GetEncryptionByIdAndPublicKey(string userId, string publicKey);
    }
}