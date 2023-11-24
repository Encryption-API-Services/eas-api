using DataLayer.Mongo.Entities;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IHashedPasswordRepository
    {
        public Task InsertOneHasedPassword(HashedPassword password);
        public Task<HashedPassword> GetOneHashedPassword(string id);
    }
}
