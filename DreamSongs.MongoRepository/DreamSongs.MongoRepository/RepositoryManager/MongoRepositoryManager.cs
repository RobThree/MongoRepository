using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DreamSongs.MongoRepository
{
    //TODO: Code coverage here is near-zero. A new RepoManagerTests.cs class needs to be created and we need to
    //      test these methods. Ofcourse we also need to update codeplex documentation on this entirely new object.
    //      This is a work-in-progress.

    //TODO: Implement GetIndexes(); figure out what we want to return (preferrably a "wrapper" around GetIndexesResult)

    //TODO: GetStats(), Validate() and EnsureIndexes(IMongoIndexKeys, IMongoIndexOptions) "leak" MongoDb-specific
    //      details. These probably need to get wrapped in MongoRepository specific objects to hide MongoDb.

    /// <summary>
    /// Deals with the collections of entities in MongoDb. This class tries to hide as much MongoDb-specific details
    /// as possible but it's not 100% yet. It is a very thin wrapper around most collection methods on MongoDb's
    /// MongoCollection objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoRepositoryManager<T> : IRepositoryManager<T>
        where T : Entity
    {
        /// <summary>
        /// MongoCollection field
        /// </summary>
        private MongoCollection<T> _collection;

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and the repository (i.e T)
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring</remarks>
        public MongoRepositoryManager()
            : this(Util.GetDefaultConnectionString())
        { }

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and the repository (i.e T)
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB</param>
        public MongoRepositoryManager(string connectionString)
        {
            _collection = Util.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Drops the collection
        /// </summary>
        public void Drop()
        {
            _collection.Drop();
        }

        /// <summary>
        /// Tests whether the repository is capped
        /// </summary>
        /// <returns>Returns true when the repository is capped, false otherwise</returns>
        public bool IsCapped()
        {
            return _collection.IsCapped();
        }

        /// <summary>
        /// Drops specified index on the repository
        /// </summary>
        /// <param name="keynames">The name of the indexed field</param>
        public void DropIndex(string keyname)
        {
            this.DropIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Drops specified indexes on the repository
        /// </summary>
        /// <param name="keynames">The names of the indexed fields</param>
        public void DropIndexes(IEnumerable<string> keynames)
        {
            _collection.DropIndex(keynames.ToArray());
        }

        /// <summary>
        /// Drops all indexes on this repository
        /// </summary>
        public void DropAllIndexes()
        {
            _collection.DropAllIndexes();
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist
        /// </summary>
        /// <param name="keynames">The indexed field</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse
        /// </remarks>
        public void EnsureIndex(string keyname)
        {
            this.EnsureIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist
        /// </summary>
        /// <param name="keynames">The indexed field</param>
        /// <param name="descending">Set to true to make index descending, false for ascending</param>
        /// <param name="unique">Set to true to ensure index enforces unique values</param>
        /// <param name="sparse">Set to true to specify the index is sparse</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public void EnsureIndex(string keyname, bool descending, bool unique, bool sparse)
        {
            this.EnsureIndexes(new string[] { keyname }, descending, unique, sparse);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist
        /// </summary>
        /// <param name="keynames">The indexed fields</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse
        /// </remarks>
        public void EnsureIndexes(IEnumerable<string> keynames)
        {
            this.EnsureIndexes(keynames, false, false, false);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist
        /// </summary>
        /// <param name="keynames">The indexed fields</param>
        /// <param name="descending">Set to true to make index descending, false for ascending</param>
        /// <param name="unique">Set to true to ensure index enforces unique values</param>
        /// <param name="sparse">Set to true to specify the index is sparse</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public void EnsureIndexes(IEnumerable<string> keynames, bool descending, bool unique, bool sparse)
        {
            var ixk = new IndexKeysBuilder();
            if (descending)
                ixk.Descending(keynames.ToArray());
            else
                ixk.Ascending(keynames.ToArray());

            this.EnsureIndexes(
                ixk,
                new IndexOptionsBuilder()
                    .SetUnique(unique)
                    .SetSparse(sparse)
            );
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist
        /// </summary>
        /// <param name="keys">The indexed fields</param>
        /// <param name="options">The index options</param>
        /// <remarks>
        /// This method allows ultimate control but does "leak" some MongoDb specific implementation details
        /// </remarks>
        public void EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options)
        {
            _collection.EnsureIndex(keys, options);
        }

        /// <summary>
        /// Tests whether indexes exist
        /// </summary>
        /// <param name="keynames">The indexed fields</param>
        /// <returns>Returns true when the indexes exist, false otherwise</returns>
        public bool IndexExists(string keyname)
        {
            return this.IndexesExists(new string[] { keyname });
        }

        /// <summary>
        /// Tests whether indexes exist
        /// </summary>
        /// <param name="keynames">The indexed fields</param>
        /// <returns>Returns true when the indexes exist, false otherwise</returns>
        public bool IndexesExists(IEnumerable<string> keynames)
        {
            return _collection.IndexExists(keynames.ToArray());
        }

        /// <summary>
        /// Runs the ReIndex command on this repository
        /// </summary>
        public void ReIndex()
        {
            _collection.ReIndex();
        }

        /// <summary>
        /// Removes all entries for this repository in the index cache used by EnsureIndex.
        /// </summary>
        /// <remarks>
        /// Call this method when you know (or suspect) that a process other than this one may
        /// have dropped one or more indexes
        /// </remarks>
        public void ResetIndexCache()
        {
            _collection.ResetIndexCache();

            _collection.GetIndexes();
        }

        /// <summary>
        /// Validates the integrity of the repository
        /// </summary>
        /// <returns>Returns a ValidateCollectionResult</returns>
        /// <remarks>You will need to reference MongoDb.Driver</remarks>
        public ValidateCollectionResult Validate()
        {
            return _collection.Validate();
        }

        /// <summary>
        /// Gets stats for this repository
        /// </summary>
        /// <returns>Returns a CollectionStatsResult</returns>
        /// <remarks>You will need to reference MongoDb.Driver</remarks>
        public CollectionStatsResult GetStats()
        {
            return _collection.GetStats();
        }

        /// <summary>
        /// Gets the total size for the repository (data + indexes)
        /// </summary>
        /// <returns>Returns total size for the repository (data + indexes)</returns>
        public long GetTotalDataSize()
        {
            return _collection.GetTotalDataSize();
        }

        /// <summary>
        /// Gets the total storage size for the repository (data + indexes)
        /// </summary>
        /// <returns>Returns total storage size for the repository (data + indexes)</returns>
        public long GetTotalStorageSize()
        {
            return _collection.GetTotalStorageSize();
        }
    }
}
