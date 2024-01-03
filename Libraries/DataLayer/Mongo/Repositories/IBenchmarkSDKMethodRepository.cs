using DataLayer.Mongo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IBenchmarkSDKMethodRepository
    {
        Task InsertSDKMethodBenchmark(BenchmarkSDKMethod method);
    }
}
