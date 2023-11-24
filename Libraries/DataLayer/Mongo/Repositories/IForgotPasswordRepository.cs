using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IForgotPasswordRepository
    {
        public Task InsertForgotPasswordAttempt(string userId, string password);
        public Task<List<string>> GetLastFivePassword(string userId);
    }
}
