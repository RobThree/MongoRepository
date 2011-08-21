using System;
using System.Linq;
using System.Linq.Expressions;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

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
        /// Uses the Default App.Config tag names to fetch the connectionString and Database name
        /// Check the DBSetting class for the App.Config tag names.
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
        /// Gets the Mongo collection (to perform advance operations)
        /// </summary>
        public MongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }
        

        /// <summary>
        /// Gets the database in being used for this repository
        /// </summary>
        public MongoDatabase DB 
        {
            get
            {
                return _db;
            }
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
        /// <param name="criteria">The expression</param>
        /// <returns>The T</returns>
        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Where(criteria).FirstOrDefault();
        }

        /// <summary>
        /// Retunrs the list of T where it matches the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>List of T</returns>
        public IQueryable<T> GetAll(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Where(criteria);
        }

        /// <summary>
        /// Retunrs the All the records of T
        /// </summary>
        /// <returns>List of T</returns>
        public IQueryable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        /// <summary>
        /// Inserts the new item in DB
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item inclduing its new ObjectId</returns>
        [Obsolete("This method will be removed in future releases. Use Add method instead of Insert")]
        public T Insert(T item)
        {
            _collection.Insert<T>(item);

            return item;
        }

        /// <summary>
        /// Adds the new item in DB
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item inclduing its new ObjectId</returns>
        public T Add(T item)
        {
            _collection.Insert<T>(item);

            return item;
        }

        /// <summary>
        /// Updates a row
        /// </summary>
        /// <param name="item">The object</param>
        /// <returns>The updated object</returns>
        public T Update(T item)
        {
            _collection.Save<T>(item);

            return item;
        }

        /// <summary>
        /// Deletes a document from db by its id
        /// </summary>
        /// <param name="objectId">The obj id</param>
        [Obsolete("This method will be removed in future releases. Use Delete method instead of Remove")]
       public void Remove(string objectId)
        {
            _collection.Remove(Query.EQ("_id", objectId));
        }

        /// <summary>
        /// Deletes a document from db by its id
        /// </summary>
        /// <param name="objectId">The obj id</param>
        public void Delete(string objectId)
        {
            _collection.Remove(Query.EQ("_id", objectId));
        }

       /// <summary>
       /// Counts the total records saved in db.
       /// </summary>
       /// <returns>Int value</returns>
       public int Count()
       {
           return _collection.Count();
       }

       /// <summary>
       /// Checks if the entity exists for given criteria
       /// </summary>
       /// <typeparam name="T">The T</typeparam>
       /// <param name="criteria">The expression</param>
       /// <returns>true or false</returns>
       public bool Exists(Expression<Func<T, bool>> criteria)
       {
           return _collection.AsQueryable().Any(criteria);
       }

        /// <summary>
        /// Returns an IQueryable for the given entity
        /// </summary>
        /// <returns>The IQueryable </returns>
        public IQueryable<T> AsQueryable()
       {
           return _collection.AsQueryable();
       }        
    }
}

