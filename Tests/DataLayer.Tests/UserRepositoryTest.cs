using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Encryption.PasswordHash;
using Models.UserAuthentication;
using MongoDB.Driver;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace DataLayer.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UserRepositoryTest
    {
        private readonly IUserRepository _userRepository;
        private readonly RegisterUser _registerUser;
        private readonly MongoClient _mongoClient;
        public UserRepositoryTest()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
            this._mongoClient = new MongoClient(settings);

            this._userRepository = new UserRepository(new DatabaseSettings
            {
                Connection = Environment.GetEnvironmentVariable("Connection"),
                DatabaseName = Environment.GetEnvironmentVariable("DatabaseName"),
                UserCollectionName = Environment.GetEnvironmentVariable("UserCollectionName")
            }, this._mongoClient);
            this._registerUser = new RegisterUser
            {
                username = "testUser1234",
                email = "testingemail@outlook.com",
                password = "Testing1234@#$1!"
            };
        }

        [Fact, Priority(-10)]
        public async Task AddUserTest()
        {
            Argon2Wrappper argon2 = new Argon2Wrappper();
            IntPtr hashedPasswordPtr = await argon2.HashPasswordAsync("DoNotUseThisPassword");
            string hashedPassword = Marshal.PtrToStringUTF8(hashedPasswordPtr);
            Argon2Wrappper.free_cstring(hashedPasswordPtr);
            await this._userRepository.DeleteUserByEmail(this._registerUser.email);
            await this._userRepository.AddUser(this._registerUser, hashedPassword);
            User databaseUser = await this._userRepository.GetUserByEmail(this._registerUser.email);
            Assert.NotNull(databaseUser);
        }

        [Fact, Priority(0)]
        public async Task GetUserByEmail()
        {
            User databaseUser = await this._userRepository.GetUserByEmail(this._registerUser.email);
            Assert.NotNull(databaseUser);
        }
    }
}