using DataLayer.Mongo.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IPriceRepository
    {
        public Task<Price> GetPriceByAmount(long amount);
        public Task InsertPrice(Price price);
        public Task<List<Price>> GetPricesByProductId(string stripeProductId);
    }
}
