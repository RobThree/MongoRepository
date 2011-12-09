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
            where T : Entity
        {
            var collectionName = ((Entity)Activator.CreateInstance((typeof(T)))).CollectionName;
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be empty for this entity");

            return Util.GetDatabaseFromConnectionString(connectionstring).GetCollection<T>(collectionName);
        }
    }
}
