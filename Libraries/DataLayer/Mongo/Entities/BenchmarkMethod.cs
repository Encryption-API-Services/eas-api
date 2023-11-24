using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLayer.Mongo.Entities
{
    public class BenchmarkMethod
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public BenchmarkMethodLogger Details { get; set; }
    }
}
