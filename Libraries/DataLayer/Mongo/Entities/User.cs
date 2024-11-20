using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string StripCustomerId { get; set; }
        public string StripCardId { get; set; }
        public string StripProductId { get; set; }
        public string ApiKey { get; set; }
        public string DevelopmentApiKey { get; set; }
        public string TokenPublicKey { get; set; }
        public BillingInformation BillingInformation { get; set; }
        public Phone2FA Phone2FA { get; set; }
        public LockedOut LockedOut { get; set; }
        public EmailActivationToken EmailActivationToken { get; set; }
        public ForgotPassword ForgotPassword { get; set; }
        public UserSubscriptionSettings UserSubscriptionSettings { get; set; }
        public InactiveUserEmail InactiveEmail { get; set; }
        public EmergencyKit EmergencyKit { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    public class BillingInformation
    {
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
    public class Phone2FA
    {
        public string PhoneNumber { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class EmailActivationToken
    {
        public string Token { get; set; }
        public string PublicKey { get; set; }
        public string SignedToken { get; set; }
        public bool WasVerified { get; set; }
    }

    public class ForgotPassword
    {
        public string Token { get; set; }
        public string PublicKey { get; set; }
        public string SignedToken { get; set; }
        public bool HasBeenReset { get; set; }
    }

    public class LockedOut
    {
        public bool IsLockedOut { get; set; }
        public string Token { get; set; }
        public string PublicKey { get; set; }
    }

    public class UserSubscriptionSettings
    {
        public bool HasTrialPeriodExpired { get; set; }
        public bool IsSubscribed { get; set; }
        public string StripSubscriptionId { get; set; }
        public byte[] SubscriptionDigitalSignature { get; set; }
        public byte[] SubscriptionPublicKey { get; set; }
    }
    public class InactiveUserEmail
    {
        public bool Sent { get; set; }
        public string Token { get; set; }
        public string PublicKey { get; set; }
    }

    public class EmergencyKit
    {
        public byte[] CipherText { get; set; }
        public string EmergencyKitId { get; set; }
        public byte[] InfoStr { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] Tag { get; set; }
    }
}