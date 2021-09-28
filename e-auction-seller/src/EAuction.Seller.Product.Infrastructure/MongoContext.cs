﻿using EAuction.Seller.Product.Domain.Aggregate;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Infrastructure
{
    public class MongoContext : IMongoContext
    {
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly MongoDbConfiguration _configuration;


        public MongoContext(MongoDbConfiguration configuration)
        {
            _configuration = configuration;


            _commands = new List<Func<Task>>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConfigureMongo();

            using (Session = await MongoClient.StartSessionAsync())
            {
                Session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync();
            }

            return _commands.Count;
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
            {
                return;
            }

            // Configure mongo (You can inject the config, just to simplify)
            MongoClient = new MongoClient(_configuration.ConnectionString);

            Database = MongoClient.GetDatabase(_configuration.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            ConfigureMongo();

            return Database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }


    }

}
