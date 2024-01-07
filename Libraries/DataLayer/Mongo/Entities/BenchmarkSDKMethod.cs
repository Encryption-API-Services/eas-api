using CASHelpers.Types.HttpResponses.BenchmarkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataLayer.Mongo.Entities
{
    public class BenchmarkSDKMethod
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public string MethodName { get; set; }
        public string? MethodDescription { get; set; }
        public DateTime MethodStart { get; set; }
        public DateTime MethodEnd { get; set; }
        public BenchmarkMethodType MethodType { get; set; }
    }
}
