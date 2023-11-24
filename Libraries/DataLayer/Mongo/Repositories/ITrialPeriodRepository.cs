using Models.TrialPeriod;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ITrialPeriodRepository
    {
        public Task Insert(TrialPeriodRequest trialPeriodRequest);
        public Task<long> GetTrialPeriodRequestsCount(string userId);
    }
}
