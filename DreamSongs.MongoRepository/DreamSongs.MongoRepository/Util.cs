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

        public static MongoCollection<T> GetCollectionFromConnectionString<T>(string connectionString)
            where T : Entity
        {
            var collectionName = ((Entity)Activator.CreateInstance((typeof(T)))).CollectionName;
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be empty for this entity");

            var cnn = new MongoUrl(connectionString);
            var _server = MongoServer.Create(cnn.ToServerSettings());
            var _db = _server.GetDatabase(cnn.DatabaseName);
            return _db.GetCollection<T>(collectionName);
        }
    }
}
