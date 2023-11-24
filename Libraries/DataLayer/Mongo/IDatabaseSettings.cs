namespace DataLayer.Mongo
{
    public interface IDatabaseSettings
    {
        public string Connection { get; set; }
        public string DatabaseName { get; set; }
        public string UserCollectionName { get; set; }
    }
}
