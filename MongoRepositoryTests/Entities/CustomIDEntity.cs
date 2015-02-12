using MongoRepository;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoRepositoryTests.Entities
{
    public class CustomIDEntity : IEntity
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    [CollectionName("MyTestCollection")]
    public class CustomIDEntityCustomCollection : CustomIDEntity { }
}
