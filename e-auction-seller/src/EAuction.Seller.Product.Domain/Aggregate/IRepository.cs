using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Domain.Aggregate
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Add(TEntity obj);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetById(string id);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAll();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Update(TEntity obj);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Remove(string id);
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> AsQueryable();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProjected"></typeparam>
        /// <param name="filterExpression"></param>
        /// <param name="projectionExpression"></param>
        /// <returns></returns>
        IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjected>> projectionExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="fieldDefinition"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task PushItemToArray<T>(string id, FieldDefinition<TEntity> fieldDefinition, T item);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity FindById(string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> FindByIdAsync(string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        void InsertOne(TEntity document);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertOneAsync(TEntity document);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        void InsertMany(ICollection<TEntity> documents);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task InsertManyAsync(ICollection<TEntity> documents);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        void ReplaceOne(TEntity document);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task ReplaceOneAsync(TEntity document);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        void DeleteOne(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void DeleteById(string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteByIdAsync(string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        void DeleteMany(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<UpdateResult> UpdateOneAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);

        /// <summary>
        /// 
        /// </summary>
        public Task StartSessionAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task AbortTransactionAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task CommitTransactionAsync();

    }

}
