using System;
using System.Linq;
using System.Linq.Expressions;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DreamSongs.MongoRepository
{
    public class MongoRepository<T> : IRepository<T> where T : Entity
    {
        /// <summary>
        /// MongoCollection field
        /// </summary>
        MongoCollection<T> _collection;

        /// <summary>
        /// MongoDB Server
        /// </summary>
        MongoServer _server;

        /// <summary>
        /// MongoDB database
        /// </summary>
        MongoDatabase _db;

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and The Collection (i.e T)
        /// </summary>
        public MongoRepository()
        {
            _server = MongoServer.Create(DBSetting.ConnectionString);
            _db = _server.GetDatabase(DBSetting.Database);

            var collectionName = ((Entity)Activator.CreateInstance((typeof(T)))).CollectionName;

            if (String.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Collection name can't be empty for this entity");
            }

            _collection = _db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The object Id</param>
        /// <returns>The Entity T</returns>
        public T GetById(string id)
        {
            return _collection.AsQueryable().Where(r => r.Id == new ObjectId(id)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the T (1 record) by the given criteria
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>The T</returns>
        public T GetSingle(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression).FirstOrDefault();
        }

        /// <summary>
        /// Retunrs the list of T where it matches the criteria
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>List of T</returns>
        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression);
        }

        /// <summary>
        /// Inserts the new item in DB
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item inclduing its new ObjectId</returns>
        public T Insert(T item)
        {
            _collection.Insert<T>(item);

            return item;
        }        

        public MongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }
    }
}

