using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{
    public class SuccessfulLogin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserAgent { get; set; }
        public bool WasThisMe { get; set; }
        public bool HasBeenChecked { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
