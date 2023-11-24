using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IEASExceptionRepository
    {
        Task InsertException(string exceptionMessage, string methodBase);
    }
}
