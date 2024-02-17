using DataLayer.Mongo.Entities;
using Models.Payments;
using Models.UserAuthentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IUserRepository
    {
        public Task<User> AddUser(RegisterUser model, string hashedPassword);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetUserByUsername(string username);
        public Task ChangeUsername(string userId, string newUsername);
        public Task<List<User>> GetUsersMadeWithinLastThirtyMinutes();
        public Task<User> GetUserById(string id);
        public Task ChangeUserActiveById(User user, bool isActive, string stripCustomerId);
        public Task UpdatePassword(string userId, string password);
        public Task LockoutUser(string userId);
        public Task UpdateForgotPassword(string userId, ForgotPassword forgotPassword);
        public Task<List<User>> GetLockedOutUsers();
        public Task<List<User>> GetUsersWhoForgotPassword();
        public Task UpdateUsersForgotPasswordToReset(string userId, string forgotPasswordToken, string publicKey, string signedToken);
        public Task UpdateUserLockedOutToSentOut(string userId);
        public Task UnlockUser(string userId);
        public Task<Phone2FA> GetPhone2FAStats(string userId);
        public Task ChangePhone2FAStatusToEnabled(string userId);
        public Task ChangePhone2FAStatusToDisabled(string userId);
        public Task ChangePhoneNumberByUserID(string userId, string phoneNumber);
        public Task<string> GetPhoneNumberByUserId(string userId);
        public Task AddCardToUser(string userId, string cardId);
        public Task<string> GetApiKeyById(string userId);
        public Task UpdateApiKeyByUserId(string userId, string newApiKey);
        public Task<User> GetUserByApiKey(string apiKey);
        public Task DeleteUserByEmail(string email);
        public Task DeleteUserByUserId(string userId);
        public Task UpdateTrialPeriodToExpired(string userId);
        public Task<List<User>> GetActiveUsers();
        public Task UpdateInactiveEmailSent(string userId, string token, string publicKey);
        public Task UpdateStripeSubscriptionAndProductId(string userId, string subscriptionId, string productId);
        public Task UpdateStripeSubscriptionToNull(string userId);
        public Task UpdateBillingInformation(string userId, UpdateBillingInformationRequestBody billingInformation);
    }
}
