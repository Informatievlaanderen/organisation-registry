namespace OrganisationRegistry.SqlServer.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class SqlServerConfiguration
    {
        public static string Section = "SqlServer";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string ConnectionString { get; set; }
        public string MigrationsConnectionString { get; set; }

        public SqlServerConfiguration Obfuscate()
        {
            return new SqlServerConfiguration
            {
                ConnectionString = Obfuscator.ObfuscateConnectionString(ConnectionString),
                MigrationsConnectionString = Obfuscator.ObfuscateConnectionString(MigrationsConnectionString)
            };
        }
    }
}
