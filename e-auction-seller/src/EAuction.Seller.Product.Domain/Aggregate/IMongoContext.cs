using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Domain.Aggregate
{
    public interface IMongoContext : IDisposable
    {
        IMongoCollection<T> GetCollection<T>(string name);
        MongoClient MongoClient { get; }


    }


}
