using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{
    public class HotpCode
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Hotp { get; set; }
        public byte[] SecretKey { get; set; }
        public long Counter { get; set; }
        public bool HasBeenVerified { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
