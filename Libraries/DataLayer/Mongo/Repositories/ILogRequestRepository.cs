using DataLayer.Mongo.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ILogRequestRepository
    {
        Task InsertRequest(LogRequest request);
        Task InsertRequests(List<LogRequest> requests);
    }
}
