namespace MongoRepository
{
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // TODO: Code coverage here is near-zero. A new RepoManagerTests.cs class needs to be created and we need to
    //      test these methods. Ofcourse we also need to update codeplex documentation on this entirely new object.
    //      This is a work-in-progress.

    // TODO: GetStats(), Validate(), GetIndexes and EnsureIndexes(IMongoIndexKeys, IMongoIndexOptions) "leak"
    //      MongoDb-specific details. These probably need to get wrapped in MongoRepository specific objects to hide
    //      MongoDb.

    /// <summary>
    /// Deals with the collections of entities in MongoDb. This class tries to hide as much MongoDb-specific details
    /// as possible but it's not 100% *yet*. It is a very thin wrapper around most methods on MongoDb's MongoCollection
    /// objects.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository to manage.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class MongoRepositoryManager<T, TKey> : IRepositoryManager<T, TKey>
        where T : IEntity<TKey>
    {
        /// <summary>
        /// MongoCollection field.
        /// </summary>
        private MongoCollection<T> collection;

        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepositoryManager()
            : this(Util<TKey>.GetDefaultConnectionString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepositoryManager(string connectionString)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepositoryManager(string connectionString, string collectionName)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString, collectionName);
        }

        /// <summary>
        /// Gets a value indicating whether the collection already exists.
        /// </summary>
        /// <value>Returns true when the collection already exists, false otherwise.</value>
        public virtual bool Exists
        {
            get { return this.collection.Exists(); }
        }

        /// <summary>
        /// Gets the name of the collection as Mongo uses.
        /// </summary>
        /// <value>The name of the collection as Mongo uses.</value>
        public virtual string Name
        {
            get { return this.collection.Name; }
        }

        /// <summary>
        /// Drops the collection.
        /// </summary>
        public virtual void Drop()
        {
            this.collection.Drop();
        }

        /// <summary>
        /// Tests whether the repository is capped.
        /// </summary>
        /// <returns>Returns true when the repository is capped, false otherwise.</returns>
        public virtual bool IsCapped()
        {
            return this.collection.IsCapped();
        }

        /// <summary>
        /// Drops specified index on the repository.
        /// </summary>
        /// <param name="keyname">The name of the indexed field.</param>
        public virtual void DropIndex(string keyname)
        {
            this.DropIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Drops specified indexes on the repository.
        /// </summary>
        /// <param name="keynames">The names of the indexed fields.</param>
        public virtual void DropIndexes(IEnumerable<string> keynames)
        {
            this.collection.DropIndex(keynames.ToArray());
        }

        /// <summary>
        /// Drops all indexes on this repository.
        /// </summary>
        public virtual void DropAllIndexes()
        {
            this.collection.DropAllIndexes();
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist.
        /// </summary>
        /// <param name="keyname">The indexed field.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        public virtual void EnsureIndex(string keyname)
        {
            this.EnsureIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist.
        /// </summary>
        /// <param name="keyname">The indexed field.</param>
        /// <param name="descending">Set to true to make index descending, false for ascending.</param>
        /// <param name="unique">Set to true to ensure index enforces unique values.</param>
        /// <param name="sparse">Set to true to specify the index is sparse.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public virtual void EnsureIndex(string keyname, bool descending, bool unique, bool sparse)
        {
            this.EnsureIndexes(new string[] { keyname }, descending, unique, sparse);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        public virtual void EnsureIndexes(IEnumerable<string> keynames)
        {
            this.EnsureIndexes(keynames, false, false, false);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <param name="descending">Set to true to make index descending, false for ascending.</param>
        /// <param name="unique">Set to true to ensure index enforces unique values.</param>
        /// <param name="sparse">Set to true to specify the index is sparse.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public virtual void EnsureIndexes(IEnumerable<string> keynames, bool descending, bool unique, bool sparse)
        {
            var ixk = new IndexKeysBuilder();
            if (descending)
            {
                ixk.Descending(keynames.ToArray());
            }
            else
            {
                ixk.Ascending(keynames.ToArray());
            }

            this.EnsureIndexes(
                ixk,
                new IndexOptionsBuilder().SetUnique(unique).SetSparse(sparse));
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keys">The indexed fields.</param>
        /// <param name="options">The index options.</param>
        /// <remarks>
        /// This method allows ultimate control but does "leak" some MongoDb specific implementation details.
        /// </remarks>
        public virtual void EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options)
        {
            this.collection.CreateIndex(keys, options);
        }

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keyname">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        public virtual bool IndexExists(string keyname)
        {
            return this.IndexesExists(new string[] { keyname });
        }

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        public virtual bool IndexesExists(IEnumerable<string> keynames)
        {
            return this.collection.IndexExists(keynames.ToArray());
        }

        /// <summary>
        /// Runs the ReIndex command on this repository.
        /// </summary>
        public virtual void ReIndex()
        {
            this.collection.ReIndex();
        }

        /// <summary>
        /// Gets the total size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total size for the repository (data + indexes).</returns>
        [Obsolete("This method will be removed in the next version of the driver")]
        public virtual long GetTotalDataSize()
        {
            return this.collection.GetTotalDataSize();
        }

        /// <summary>
        /// Gets the total storage size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total storage size for the repository (data + indexes).</returns>
        [Obsolete("This method will be removed in the next version of the driver")]
        public virtual long GetTotalStorageSize()
        {
            return this.collection.GetTotalStorageSize();
        }

        /// <summary>
        /// Validates the integrity of the repository.
        /// </summary>
        /// <returns>Returns a ValidateCollectionResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        public virtual ValidateCollectionResult Validate()
        {
            return this.collection.Validate();
        }

        /// <summary>
        /// Gets stats for this repository.
        /// </summary>
        /// <returns>Returns a CollectionStatsResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        public virtual CollectionStatsResult GetStats()
        {
            return this.collection.GetStats();
        }

        /// <summary>
        /// Gets the indexes for this repository.
        /// </summary>
        /// <returns>Returns the indexes for this repository.</returns>
        public virtual GetIndexesResult GetIndexes()
        {
            return this.collection.GetIndexes();
        }
    }

    /// <summary>
    /// Deals with the collections of entities in MongoDb. This class tries to hide as much MongoDb-specific details
    /// as possible but it's not 100% *yet*. It is a very thin wrapper around most methods on MongoDb's MongoCollection
    /// objects.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository to manage.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class MongoRepositoryManager<T> : MongoRepositoryManager<T, string>, IRepositoryManager<T>
        where T : IEntity<string>
    {
        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepositoryManager()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepositoryManager(string connectionString)
            : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepositoryManager class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepositoryManager(string connectionString, string collectionName)
            : base(connectionString, collectionName) { }
    }
}
