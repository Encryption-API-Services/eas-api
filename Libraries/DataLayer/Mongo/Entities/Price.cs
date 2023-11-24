using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{
    public class Price
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long Amount { get; set; }
        public string StripeId { get; set; }
        public string StripeProductId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
