using DataLayer.Mongo.Entities;
using System.Collections.Generic;

namespace DataLayer.Mongo.CustomEntities
{
    public class ProductWithPrices
    {
        public Product Product { get; set; }
        public List<Price> Prices { get; set; }
        public bool IsAssignedToMe { get; set; }
    }
}
