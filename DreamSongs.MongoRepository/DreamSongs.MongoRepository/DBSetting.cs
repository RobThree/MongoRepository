using System;
using System.Configuration;

namespace DreamSongs.MongoRepository
{
    public static class DBSetting
    {
        /// <summary>
        /// Constring field
        /// </summary>
        private static string _connString;

        /// <summary>
        /// placeholder for server settings tag in app/web.config (use to build the connectionstring)
        /// </summary>
        private static string _settingsTag = "MongoServerSettings";

        /// <summary>
        /// the database tag name in app/web.config (use to build the connectionstring)
        /// </summary>
        private static string _dbNameTag = "MongoDBName";

        /// <summary>
        /// The database (in use) name
        /// </summary>
        private static string _database;

        /// <summary>
        /// Gets or sets the server name
        /// </summary>
        public static string Server { get; set; }

        /// <summary>
        /// Gets or sets the port on which MongoDB server is bind to.
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name for the db.
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the db.
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// Gets the Database settings from webconfig
        /// </summary>
        public static string Database 
        { 
            get
            {
               if (string.IsNullOrEmpty(_database))
                {
                    var db = ConfigurationManager.AppSettings[_dbNameTag];
                    _database = db;                    
                }               

                return _database;
            }

            set
            {
                _database = value;
            }
        }

        /// <summary>
        /// Gets the conncetion string
        /// </summary>
        public static string ConnectionString
        {

            get
            {
                if (string.IsNullOrEmpty(_connString))
                {
                    var settings = ConfigurationManager.AppSettings[_settingsTag];
                    if (string.IsNullOrEmpty(settings))
                    {
                        string authentication = string.Empty;
                        if (Username != null)
                        {
                            authentication = string.Concat(Username, ':', Password, '@');
                        }

                        _connString = string.Format("mongodb://{0}{1}:{2}/{3}", authentication, Server, Port, Database);
                    }
                    else
                    {
                        _connString = settings;
                    }                    
                }

                return _connString;
            }

        }
    }
}