using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ICASExceptionRepository
    {
        Task InsertException(string exceptionMessage, string methodBase);
    }
}
