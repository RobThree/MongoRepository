using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoRepository
{
    public static class GlobalConfig
    {
        private static Action<SslSettings> ConfigureSslSettingsAction { get; set; }

        public static void ConfigureSslSettings(Action<SslSettings> configureSslSettings)
        {
            ConfigureSslSettingsAction = configureSslSettings;
        }

        internal static SslSettings GetSslSettings(SslSettings settings)
        {
            if (settings == null)
            {
                settings = new SslSettings();
            }

            ConfigureSslSettingsAction?.Invoke(settings);

            return settings;
        }
    }
}
