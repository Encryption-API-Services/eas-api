using CasDotnetSdk.PasswordHashers;
using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Models.UserAuthentication;
using MongoDB.Driver;
using System;
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
            Argon2Wrapper argon2 = new Argon2Wrapper();
            string hashedPassword = argon2.HashPassword("DoNotUseThisPassword");
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