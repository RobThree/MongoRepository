using System;
using System.Configuration;
using MongoDB.Driver;

namespace DreamSongs.MongoRepository
{
    internal static class Util
    {
        private const string DefaultConnectionstringName = "MongoServerSettings";

        public static string GetDefaultConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[DefaultConnectionstringName].ConnectionString;
        }

        public static MongoDatabase GetDatabaseFromConnectionString(string connectionstring)
        {
            var cnn = new MongoUrl(connectionstring);
            var _server = MongoServer.Create(cnn.ToServerSettings());
            return _server.GetDatabase(cnn.DatabaseName);
        }

        public static MongoCollection<T> GetCollectionFromConnectionString<T>(string connectionstring)
            where T : IEntity
        {
            string collectionName;
            if (typeof(T).BaseType.Equals(typeof(System.Object)))
                collectionName = GetCollectioNameFromInterface<T>();
            else
                collectionName = GetCollectioNameFromType(typeof(T));

            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be empty for this entity");

            return Util.GetDatabaseFromConnectionString(connectionstring).GetCollection<T>(collectionName);
        }

        private static string GetCollectioNameFromInterface<T>()
        {
            string collectionname;

            //Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(typeof(T), typeof(CollectionName));
            if (att != null)
                //It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionName)att).Name;
            else
                collectionname = typeof(T).Name;
            return collectionname;
        }


        private static string GetCollectioNameFromType(Type entitytype)
        {
            string collectionname;

            //Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(entitytype, typeof(CollectionName));
            if (att != null)
                //It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionName)att).Name;
            else
            {
                //No attribute found, get the basetype
                while (!entitytype.BaseType.Equals(typeof(Entity)))
                    entitytype = entitytype.BaseType;
                collectionname = entitytype.Name;
            }
            return collectionname;
        }

    }
}
