using DataLayer.Mongo.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface ICreditCardInfoChangedRepository
    {
        Task InsertCreditCardInformationChanged(CreditCardInfoChanged changedInfo);
        Task<List<CreditCardInfoChanged>> GetUnsentNotifications();
        Task UpdateInfoToSent(CreditCardInfoChanged changedInfo);
    }
}