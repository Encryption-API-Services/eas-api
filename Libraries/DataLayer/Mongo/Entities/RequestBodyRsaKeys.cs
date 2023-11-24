using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLayer.Mongo.Entities
{
    public class RequestBodyRsaKeys
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClientPublicKey { get; set; }
        public string ClientPrivateKey { get; set; }
        public string AesKey { get; set; }
        public string Nonce { get; set; }
    }
}