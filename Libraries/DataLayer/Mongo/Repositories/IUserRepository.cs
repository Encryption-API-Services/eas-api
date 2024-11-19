using Common.EmergencyKit;
using DataLayer.Mongo.Entities;
using Models.Payments;
using Models.UserAdmin;
using Models.UserAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IUserRepository
    {
        public Task ChangeUserActivationStatusById(string userId, bool isActive);
        public Task ChangeUserAdminStatusById(string userId, bool isAdmin);
        public IQueryable<UserTableItem> GetUsersByPage();
        public Task<User> AddUser(RegisterUser model, string hashedPassword);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetUserByUsername(string username);
        public Task ChangeUsername(string userId, string newUsername);
        public Task<User> GetUserById(string id);
        public Task ChangeUserActiveById(User user, bool isActive, string stripCustomerId);
        public Task UpdatePassword(string userId, string password);
        public Task LockoutUser(string userId);
        public Task UpdateUsersForgotPasswordToReset(string userId, string forgotPasswordToken, string publicKey, string signedToken);
        public Task UnlockUser(string userId);
        public Task<Phone2FA> GetPhone2FAStats(string userId);
        public Task ChangePhone2FAStatusToEnabled(string userId);
        public Task ChangePhone2FAStatusToDisabled(string userId);
        public Task ChangePhoneNumberByUserID(string userId, string phoneNumber);
        public Task<string> GetPhoneNumberByUserId(string userId);
        public Task AddCardToUser(string userId, string cardId);
        public Task<Tuple<string, string>> GetApiKeysById(string userId);
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
        public Task UpdateUsersRsaKeyPairsAndToken(string userId, string pubXml, string token, string signedToken);
        public Task UpdateLockedOutUsersToken(string userId, string lockedOutToken, string publicKey);
        public Task SetEmergencyKitForUser(string userId, EmergencyKitCreatedResult kit);
        public Task<EmergencyKit> GetEmergencyKitByEmail(string email);
        public Task SetUserTokenPublicKey(string userId, string publicKey);
        public Task<string> GetUserTokenPublicKey(string userId);
    }
}
