using EAuction.Auction.Domain.Aggregate;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.Auction.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : IAggregateRoot
    {
        protected readonly IMongoContext Context;
        protected IMongoCollection<TEntity> DbSet;
        protected IClientSessionHandle Session;

        protected BaseRepository(IMongoContext context)
        {
            Context = context;
            DbSet = Context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.InsertOneAsync(Session, obj);
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            var data = await DbSet.FindAsync(Session, Builders<TEntity>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Session, Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.ReplaceOneAsync(Session, Builders<TEntity>.Filter.Eq("_id", obj.Id), obj);
        }

        public virtual void Remove(string id)
        {
            DbSet.DeleteOneAsync(Session, Builders<TEntity>.Filter.Eq("_id", id));
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return DbSet.AsQueryable<TEntity>(Session);
        }

        public virtual IEnumerable<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DbSet.Find(Session, filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression)
        {
            return DbSet.Find(Session, filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DbSet.Find(Session, filterExpression).FirstOrDefault();
        }

        public virtual Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => DbSet.Find(Session, filterExpression).FirstOrDefaultAsync());
        }

        public virtual TEntity FindById(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, id);
            return DbSet.Find(Session, filter).SingleOrDefault();
        }

        public virtual Task<TEntity> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, id);
                return DbSet.Find(Session, filter).SingleOrDefaultAsync();
            });
        }

        public async Task<UpdateResult> UpdateOneAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            return await DbSet.UpdateOneAsync(Session, filter, update);
        }

        public virtual void InsertOne(TEntity document)
        {
            DbSet.InsertOne(Session, document);
        }

        public virtual Task InsertOneAsync(TEntity document)
        {
            return Task.Run(() => DbSet.InsertOneAsync(Session, document));
        }

        public void InsertMany(ICollection<TEntity> documents)
        {
            DbSet.InsertMany(Session, documents);
        }


        public virtual async Task InsertManyAsync(ICollection<TEntity> documents)
        {
            await DbSet.InsertManyAsync(Session, documents);
        }

        public void ReplaceOne(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            DbSet.FindOneAndReplace(Session, filter, document);
        }

        public virtual async Task ReplaceOneAsync(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            await DbSet.FindOneAndReplaceAsync(Session, filter, document);
        }

        public void DeleteOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            DbSet.FindOneAndDelete(Session, filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => DbSet.FindOneAndDeleteAsync(Session, filterExpression));
        }

        public void DeleteById(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, id);
            DbSet.FindOneAndDelete(Session, filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, id);
                DbSet.FindOneAndDeleteAsync(Session, filter);
            });
        }

        public void DeleteMany(Expression<Func<TEntity, bool>> filterExpression)
        {
            DbSet.DeleteMany(Session, filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => DbSet.DeleteManyAsync(Session, filterExpression));
        }

        public async Task PushItemToArray<T>(string id, FieldDefinition<TEntity> fieldDefinition, T item)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var update = Builders<TEntity>.Update
                    .Push(fieldDefinition, item);
            await DbSet.FindOneAndUpdateAsync(Session, filter, update);
        }

        public async Task StartSessionAsync()
        {
            if (Session == null)
                Session = await Context.MongoClient.StartSessionAsync();
        }

        public async Task AbortTransactionAsync()
        {
            if (Session != null)
                await Session.AbortTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (Session != null)
                await Session.CommitTransactionAsync();
        }
    }
}
