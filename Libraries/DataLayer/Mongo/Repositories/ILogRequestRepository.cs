using DataLayer.Mongo.Entities;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ILogRequestRepository
    {
        Task InsertRequest(LogRequest request);
    }
}
