using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Text_Service
{
    public class TwoFactorAuthHotpCode
    {
        private readonly IDatabaseSettings _databaseSettings;
        private readonly MongoClient _mongoClient;
        private readonly string accountSid;
        private readonly string authToken;
        public TwoFactorAuthHotpCode(IDatabaseSettings databaseSettings, MongoClient mongoClient)
        {
            this._databaseSettings = databaseSettings;
            this._mongoClient = mongoClient;
            this.accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            this.authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        }

        public async Task GetHotpCodesToSendOut()
        {
            IHotpCodesRepository hotpCodesRepository = new HotpCodesRepository(this._databaseSettings, this._mongoClient);
            IUserRepository userRepository = new UserRepository(this._databaseSettings, this._mongoClient);
            List<HotpCode> codes = await hotpCodesRepository.GetAllHotpCodesNotSent();
            for (int i = 0; i < codes.Count; i++)
            {
                string phoneNumberToSendToo = await userRepository.GetPhoneNumberByUserId(codes[i].UserId);
                await this.SendOutHotpCode(codes[i], phoneNumberToSendToo);
                await hotpCodesRepository.UpdateHotpCodeToSent(codes[i].Id);
            }
        }
        public async Task SendOutHotpCode(HotpCode code, string phoneNumber)
        {
            TwilioClient.Init(this.accountSid, this.authToken);
            var message = MessageResource.Create(
                body: "Your Encryption API Services token is: " + code.Hotp,
                from: new Twilio.Types.PhoneNumber("+15627844347"),
                to: new Twilio.Types.PhoneNumber("+1" + phoneNumber)
            );
        }
    }
}
