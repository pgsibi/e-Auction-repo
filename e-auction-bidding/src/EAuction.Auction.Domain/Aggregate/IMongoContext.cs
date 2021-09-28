using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Domain.Aggregate
{
    public interface IMongoContext : IDisposable
    {
        IMongoCollection<T> GetCollection<T>(string name);
        MongoClient MongoClient { get; }

    }
}
