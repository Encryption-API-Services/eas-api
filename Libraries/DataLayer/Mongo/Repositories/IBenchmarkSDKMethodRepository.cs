using DataLayer.Mongo.Entities;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IBenchmarkSDKMethodRepository
    {
        Task InsertSDKMethodBenchmark(BenchmarkSDKMethod method);
    }
}
