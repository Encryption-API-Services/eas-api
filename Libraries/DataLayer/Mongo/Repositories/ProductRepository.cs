using DataLayer.Mongo.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;
        public ProductRepository(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            this._productCollection = database.GetCollection<Product>("Products");
        }
        public async Task InsertProduct(Product newProduct)
        {
            await this._productCollection.InsertOneAsync(newProduct);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await this._productCollection.Find(product => true).ToListAsync();
        }

        public async Task<Product> GetProductByName(string productName)
        {
            return await this._productCollection.Find(x => x.ProductName == productName).FirstOrDefaultAsync();
        }
    }
}