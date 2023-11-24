using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Common.TwilioHelpers
{
    public class MessageSender
    {
        public void SendHotpCode(string toPhoneNumber, string hotpCode)
        {
            string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var message = MessageResource.Create(
                body: "Encryption API Services - Two Facotr Authentication Login Code: " + hotpCode,
                from: new Twilio.Types.PhoneNumber("+15627844347"),
                to: new Twilio.Types.PhoneNumber("+1" + toPhoneNumber)
            );
            TwilioClient.Init(accountSid, authToken);
        }
    }
}
