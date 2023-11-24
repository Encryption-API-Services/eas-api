using System;

namespace DataLayer.Mongo
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string Connection { get; set; }
        public string DatabaseName { get; set; }
        public string UserCollectionName { get; set; }

        public DatabaseSettings()
        {
            this.Connection = Environment.GetEnvironmentVariable("Connection");
            this.DatabaseName = Environment.GetEnvironmentVariable("DatabaseName");
            this.UserCollectionName = Environment.GetEnvironmentVariable("UserCollectionName");
        }
    }
}
