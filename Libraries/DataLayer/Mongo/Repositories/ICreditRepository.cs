using DataLayer.Mongo.Entities;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ICreditRepository
    {
        public Task AddValidatedCreditInformation(ValidatedCreditCard card);
    }
}
