namespace MongoRepository
{
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents collection system flags.
    /// </summary>
    [Flags]
    public enum CollectionSystemFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,
        /// <summary>
        /// The collection has an _id index.
        /// </summary>
        HasIdIndex = 1 // called HaveIdIndex in the server but renamed here to follow .NET naming conventions
    }

    /// <summary>
    /// Represents collection user flags.
    /// </summary>
    [Flags]
    public enum CollectionUserFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,
        /// <summary>
        /// User power of 2 size.
        /// </summary>
        UsePowerOf2Sizes = 1
    }

    /// <summary>
    /// Represents the results of the collection stats command.
    /// </summary>
    [Serializable]
    public class CollectionStatsResult
    {
        // private fields
        private IndexSizesResult _indexSizes;
        private BsonDocument _response;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionStatsResult"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public CollectionStatsResult(BsonDocument response)
        {
            _response = response;
        }

        // public properties
        /// <summary>
        /// Gets the average object size.
        /// </summary>
        public double AverageObjectSize
        {
            get { return _response["avgObjSize"].ToDouble(); }
        }

        /// <summary>
        /// Gets the data size.
        /// </summary>
        public long DataSize
        {
            get { return _response["size"].ToInt64(); }
        }

        /// <summary>
        /// Gets the extent count.
        /// </summary>
        public int ExtentCount
        {
            get { return _response["numExtents"].ToInt32(); }
        }

        /// <summary>
        /// Gets the index count.
        /// </summary>
        public int IndexCount
        {
            get { return _response["nindexes"].ToInt32(); }
        }

        /// <summary>
        /// Gets the index sizes.
        /// </summary>
        public IndexSizesResult IndexSizes
        {
            get
            {
                if (_indexSizes == null)
                {
                    // can't initialize indexSizes in the constructor because at that time the document is still empty
                    _indexSizes = new IndexSizesResult(_response["indexSizes"].AsBsonDocument);
                }
                return _indexSizes;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is capped.
        /// </summary>
        public bool IsCapped
        {
            get { return _response.GetValue("capped", false).ToBoolean(); }
        }

        /// <summary>
        /// Gets the last extent size.
        /// </summary>
        public long LastExtentSize
        {
            get { return _response["lastExtentSize"].ToInt64(); }
        }

        /// <summary>
        /// Gets the index count.
        /// </summary>
        public long MaxDocuments
        {
            get { return _response.GetValue("max", 0).ToInt32(); }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        public string Namespace
        {
            get { return _response["ns"].AsString; }
        }

        /// <summary>
        /// Gets the object count.
        /// </summary>
        public long ObjectCount
        {
            get { return _response["count"].ToInt64(); }
        }

        /// <summary>
        /// Gets the padding factor.
        /// </summary>
        public double PaddingFactor
        {
            get { return _response["paddingFactor"].ToDouble(); }
        }

        /// <summary>
        /// Gets the storage size.
        /// </summary>
        public long StorageSize
        {
            get { return _response["storageSize"].ToInt64(); }
        }

        /// <summary>
        /// Gets the system flags.
        /// </summary>
        public CollectionSystemFlags SystemFlags
        {
            get
            {
                // systemFlags was first introduced in server version 2.2 (check "flags" also for compatibility with older servers)
                BsonValue systemFlags;
                if (_response.TryGetValue("systemFlags", out systemFlags) || _response.TryGetValue("flags", out systemFlags))
                {
                    return (CollectionSystemFlags)systemFlags.ToInt32();
                }
                else
                {
                    return CollectionSystemFlags.HasIdIndex;
                }
            }
        }

        /// <summary>
        /// Gets the total index size.
        /// </summary>
        public long TotalIndexSize
        {
            get { return _response["totalIndexSize"].ToInt64(); }
        }

        /// <summary>
        /// Gets the user flags.
        /// </summary>
        public CollectionUserFlags UserFlags
        {
            get
            {
                // userFlags was first introduced in server version 2.2
                BsonValue userFlags;
                if (_response.TryGetValue("userFlags", out userFlags))
                {
                    return (CollectionUserFlags)userFlags.ToInt32();
                }
                else
                {
                    return CollectionUserFlags.None;
                }
            }
        }

        // nested classes
        /// <summary>
        /// Represents a collection of index sizes.
        /// </summary>
        public class IndexSizesResult
        {
            // private fields
            private BsonDocument _indexSizes;

            // constructors
            /// <summary>
            /// Initializes a new instance of the IndexSizesResult class.
            /// </summary>
            /// <param name="indexSizes">The index sizes document.</param>
            public IndexSizesResult(BsonDocument indexSizes)
            {
                _indexSizes = indexSizes;
            }

            // indexers
            /// <summary>
            /// Gets the size of an index.
            /// </summary>
            /// <param name="indexName">The name of the index.</param>
            /// <returns>The size of the index.</returns>
            public long this[string indexName]
            {
                get { return _indexSizes[indexName].ToInt64(); }
            }

            // public properties
            /// <summary>
            /// Gets the count of indexes.
            /// </summary>
            public int Count
            {
                get { return _indexSizes.ElementCount; }
            }

            /// <summary>
            /// Gets the names of the indexes.
            /// </summary>
            public IEnumerable<string> Keys
            {
                get { return _indexSizes.Names; }
            }

            /// <summary>
            /// Gets the sizes of the indexes.
            /// </summary>
            public IEnumerable<long> Values
            {
                get { return _indexSizes.Values.Select(v => v.ToInt64()); }
            }

            // public methods
            /// <summary>
            /// Tests whether the results contain the size of an index.
            /// </summary>
            /// <param name="indexName">The name of the index.</param>
            /// <returns>True if the results contain the size of the index.</returns>
            public bool ContainsKey(string indexName)
            {
                return _indexSizes.Contains(indexName);
            }
        }
    }
}
