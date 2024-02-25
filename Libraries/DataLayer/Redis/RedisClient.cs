using StackExchange.Redis;
using System;

namespace DataLayer.Redis
{
    public class RedisClient : IRedisClient
    {
        private readonly IDatabase _database;
        public RedisClient()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            this._database = redis.GetDatabase();
        }

        public void SetString(string key, string value)
        {
            this._database.StringSet(key, value);
        }

        public string GetString(string key)
        {
            return this._database.StringGet(key);
        }
    }
}
