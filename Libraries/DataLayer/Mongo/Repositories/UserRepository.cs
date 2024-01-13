using Common.UniqueIdentifiers;
using DataLayer.Mongo.Entities;
using Models.Payments;
using Models.UserAuthentication;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
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
                    HasBeenSentOut = false
                },
                ApiKey = await new Generator().CreateApiKey(),
                EmailActivationToken = new EmailActivationToken()
                {
                    WasVerified = false,
                    WasSent = false
                },
                UserSubscriptionSettings = new UserSubscriptionSettings()
                {
                    HasTrialPeriodExpired = false,
                    IsSubscribed = false
                },
                InactiveEmail = new InactiveUserEmail()
                {
                    Sent = false
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
        public async Task<List<User>> GetUsersMadeWithinLastThirtyMinutes()
        {
            DateTime now = DateTime.UtcNow;
            return await this._userCollection.FindAsync(x => x.IsActive == false &&
                                                        x.CreationTime < now && x.CreationTime > now.AddMinutes(-30)
                                                        && x.EmailActivationToken.WasVerified == false && x.EmailActivationToken.WasSent == false).Result.ToListAsync();
        }
        public async Task UpdateUsersRsaKeyPairsAndToken(User user, string pubXml, string token, string signedToken)
        {
            EmailActivationToken emailToken = new EmailActivationToken()
            {
                PublicKey = pubXml,
                SignedToken = signedToken,
                Token = token,
                WasVerified = false,
                WasSent = true
            };
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
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
        public async Task UpdatePassword(string userId, string password)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Password, password)
                                              .Set(x => x.ForgotPassword, null);
            await this._userCollection.UpdateOneAsync(filter, update);
        }
        public async Task UpdateForgotPassword(string userId, ForgotPassword forgotPassword)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.ForgotPassword, forgotPassword);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<User>> GetUsersWhoForgotPassword()
        {
            return await this._userCollection.Find(x => x.ForgotPassword != null &&
                                                            x.ForgotPassword.Token != null &&
                                                            x.ForgotPassword.PublicKey == null &&
                                                            x.ForgotPassword.HasBeenReset == false).ToListAsync();
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
        public async Task<List<User>> GetLockedOutUsers()
        {
            return await this._userCollection.Find(x => x.LockedOut.IsLockedOut == true && x.LockedOut.HasBeenSentOut == false).ToListAsync();
        }
        public async Task UpdateUserLockedOutToSentOut(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LockedOut.HasBeenSentOut, true);
            await this._userCollection.UpdateOneAsync(filter, update);
        }
        public async Task UnlockUser(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LockedOut.IsLockedOut, false)
                                              .Set(x => x.LockedOut.HasBeenSentOut, false);
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

        public async Task<string> GetApiKeyById(string userId)
        {
            return await this._userCollection.AsQueryable().Where(x => x.Id == userId).Select(x => x.ApiKey).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            return await this._userCollection.Find(x => x.ApiKey == apiKey && x.IsActive == true).FirstOrDefaultAsync();
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
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.UserSubscriptionSettings.StripSubscriptionId, subscriptionId)
                                              .Set(x => x.UserSubscriptionSettings.IsSubscribed, true)
                                              .Set(x => x.StripProductId, productId);
            await this._userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateStripeSubscriptionToNull(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.UserSubscriptionSettings.StripSubscriptionId, null)
                                              .Set(x => x.UserSubscriptionSettings.IsSubscribed, false)
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
    }
}