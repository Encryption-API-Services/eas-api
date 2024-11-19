using CasDotnetSdk.DigitalSignature;
using CasDotnetSdk.DigitalSignature.Types;
using Common.EmergencyKit;
using Common.UniqueIdentifiers;
using DataLayer.Mongo.Entities;
using Models.Payments;
using Models.UserAdmin;
using Models.UserAuthentication;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._userCollection = database.GetCollection<User>("Users");
        }
        public async Task<User> AddUser(RegisterUser model, string hashedPassword)
        {
            Generator generator = new Generator();
            User newUser = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = model.username,
                Password = hashedPassword,
                Email = model.email,
                IsActive = false,
                BillingInformation = new BillingInformation()
                {
                    AddressOne = model.AddressOne,
                    AddressTwo = model.AddressTwo,
                    City = model.City,
                    State = model.State,
                    Zip = model.Zip,
                    Country = model.Country
                },
                Phone2FA = new Phone2FA()
                {
                    PhoneNumber = null,
                    IsEnabled = false
                },
                CreationTime = DateTime.UtcNow,
                LastModifiedTime = DateTime.UtcNow,
                LockedOut = new LockedOut()
                {
                    IsLockedOut = false,
                },
                ApiKey = generator.CreateApiKey(),
                DevelopmentApiKey = generator.CreateApiKey(),
                EmailActivationToken = new EmailActivationToken()
                {
                    WasVerified = false
                },
                UserSubscriptionSettings = new UserSubscriptionSettings()
                {
                    HasTrialPeriodExpired = false,
                    IsSubscribed = false
                },
                InactiveEmail = new InactiveUserEmail()
                {
                    Sent = false
                },
                EmergencyKit = new EmergencyKit()
                {

                }
            };
            await this._userCollection.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<User> GetUserById(string id)
        {
            return await this._userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await this._userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        }
        public async Task UpdateUsersRsaKeyPairsAndToken(string userId, string pubXml, string token, string signedToken)
        {
            EmailActivationToken emailToken = new EmailActivationToken()
            {
                PublicKey = pubXml,
                SignedToken = signedToken,
                Token = token,
                WasVerified = false
            };
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.EmailActivationToken, emailToken);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangeUserActiveById(User user, bool isActive, string stripCustomerId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var update = Builders<User>.Update.Set(x => x.IsActive, isActive)
                                              .Set(x => x.StripCustomerId, stripCustomerId)
                                              .Set(x => x.EmailActivationToken.WasVerified, true)
                                              .Set(x => x.EmailActivationToken.Token, null)
                                              .Set(x => x.EmailActivationToken.SignedToken, null)
                                              .Set(x => x.EmailActivationToken.PublicKey, null);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangeUserActivationStatusById(string userId, bool isActive)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.IsActive, isActive);
            await this._userCollection.UpdateOneAsync(filter, update);
        }
        public async Task UpdatePassword(string userId, string password)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Password, password)
                                              .Set(x => x.ForgotPassword, null);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateUsersForgotPasswordToReset(string userId, string forgotPasswordToken, string publicKey, string signedToken)
        {
            ForgotPassword forgotPassword = new ForgotPassword()
            {
                Token = forgotPasswordToken,
                PublicKey = publicKey,
                SignedToken = signedToken,
                HasBeenReset = true
            };

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.ForgotPassword, forgotPassword);
            await this._userCollection.UpdateOneAsync(filter, update);
        }
        public async Task LockoutUser(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LockedOut.IsLockedOut, true);
            await this._userCollection.UpdateOneAsync(filter, update);
        }
        public async Task UnlockUser(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LockedOut.IsLockedOut, false)
                                              .Set(x => x.LockedOut.Token, null)
                                              .Set(x => x.LockedOut.PublicKey, null);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await this._userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<Phone2FA> GetPhone2FAStats(string userId)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Id == userId).Select(x => x.Phone2FA).FirstOrDefaultAsync();
        }

        public async Task ChangePhone2FAStatusToEnabled(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Phone2FA.IsEnabled, true);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangePhone2FAStatusToDisabled(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Phone2FA.IsEnabled, false);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangePhoneNumberByUserID(string userId, string phoneNumber)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Phone2FA.PhoneNumber, phoneNumber);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<string> GetPhoneNumberByUserId(string userId)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Id == userId).Select(x => x.Phone2FA.PhoneNumber).FirstOrDefaultAsync();
        }

        public async Task AddCardToUser(string userId, string cardId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.StripCardId, cardId)
                                              .Set(x => x.UserSubscriptionSettings.IsSubscribed, true);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Tuple<string, string>> GetApiKeysById(string userId)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Id == userId).Select(x => new Tuple<string, string>(x.ApiKey, x.DevelopmentApiKey)).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            return await this._userCollection.Find(x => (x.ApiKey == apiKey || x.DevelopmentApiKey == apiKey) && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task DeleteUserByEmail(string email)
        {
            await this._userCollection.DeleteOneAsync(x => x.Email == email);
        }

        public async Task DeleteUserByUserId(string userId)
        {
            await this._userCollection.DeleteOneAsync(x => x.Id == userId);
        }

        public async Task UpdateTrialPeriodToExpired(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.UserSubscriptionSettings.HasTrialPeriodExpired, true);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<User>> GetActiveUsers()
        {
            return await this._userCollection.Find(x => x.IsActive == true && x.InactiveEmail.Sent == false).ToListAsync();
        }

        public async Task UpdateInactiveEmailSent(string userId, string token, string publicKey)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.InactiveEmail.Sent, true)
                                              .Set(x => x.InactiveEmail.Token, token)
                                              .Set(x => x.InactiveEmail.PublicKey, publicKey);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateStripeSubscriptionAndProductId(string userId, string subscriptionId, string productId)
        {
            SHA512DigitalSignatureWrapper digitalSignatureWrap = new SHA512DigitalSignatureWrapper();
            SHAED25519DalekDigitialSignatureResult result = digitalSignatureWrap.CreateED25519(Encoding.UTF8.GetBytes(productId));
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.UserSubscriptionSettings.StripSubscriptionId, subscriptionId)
                                              .Set(x => x.UserSubscriptionSettings.IsSubscribed, true)
                                              .Set(x => x.UserSubscriptionSettings.SubscriptionDigitalSignature, result.Signature)
                                              .Set(x => x.UserSubscriptionSettings.SubscriptionPublicKey, result.PublicKey)
                                              .Set(x => x.StripProductId, productId);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateStripeSubscriptionToNull(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.UserSubscriptionSettings.StripSubscriptionId, null)
                                              .Set(x => x.UserSubscriptionSettings.IsSubscribed, false)
                                              .Set(x => x.UserSubscriptionSettings.SubscriptionDigitalSignature, null)
                                              .Set(x => x.UserSubscriptionSettings.SubscriptionPublicKey, null)
                                              .Set(x => x.StripProductId, null);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateBillingInformation(string userId, UpdateBillingInformationRequestBody billingInformation)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.BillingInformation.AddressOne, billingInformation.AddressOne)
                                              .Set(x => x.BillingInformation.AddressTwo, billingInformation.AddressTwo)
                                              .Set(x => x.BillingInformation.City, billingInformation.City)
                                              .Set(x => x.BillingInformation.State, billingInformation.State)
                                              .Set(x => x.BillingInformation.Zip, billingInformation.Zip)
                                              .Set(x => x.BillingInformation.Country, billingInformation.Country);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangeUsername(string userId, string username)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Username, username);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateApiKeyByUserId(string userId, string newApiKey)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.ApiKey, newApiKey);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateLockedOutUsersToken(string userId, string lockedOutToken, string publicKey)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LockedOut.Token, lockedOutToken)
                                              .Set(x => x.LockedOut.PublicKey, publicKey);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public IQueryable<UserTableItem> GetUsersByPage()
        {
            return this._userCollection.AsQueryable<User>().Select(x => new UserTableItem
            {
                UserId = x.Id,
                Email = x.Email,
                Username = x.Username,
                IsActive = x.IsActive,
                IsAdmin = x.IsAdmin
            });
        }

        public async Task ChangeUserAdminStatusById(string userId, bool isAdmin)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.IsAdmin, isAdmin);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetEmergencyKitForUser(string userId, EmergencyKitCreatedResult kit)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                                            .Set(x => x.EmergencyKit.InfoStr, kit.InfoStr)
                                            .Set(x => x.EmergencyKit.CipherText, kit.CipherText)
                                            .Set(x => x.EmergencyKit.Tag, kit.Tag)
                                            .Set(x => x.EmergencyKit.EmergencyKitId, kit.EmergencyKitId)
                                            .Set(x => x.EmergencyKit.PrivateKey, kit.PrivateKey);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<EmergencyKit> GetEmergencyKitByEmail(string email)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Email == email).Select(x => x.EmergencyKit).FirstOrDefaultAsync();
        }

        public async Task SetUserTokenPublicKey(string userId, string publicKey)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.TokenPublicKey, publicKey);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<string> GetUserTokenPublicKey(string userId)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Id == userId).Select(x => x.TokenPublicKey).FirstOrDefaultAsync();
        }
    }
}