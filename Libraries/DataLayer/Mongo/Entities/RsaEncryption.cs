using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{
    public class RsaEncryption
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}