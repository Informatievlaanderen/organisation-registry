namespace OrganisationRegistry.KboMutations.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class KboMutationsConfiguration
    {
        public static string Section = "KboMutations";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SourcePath { get; set; } = null!;
        public string CachePath { get; set; } = null!;

        public string CertPath { get; set; } = null!;
        public string CaCertPath { get; set; } = null!;
        public string KeyPath { get; set; } = null!;
        public string KeyType { get; set; } = null!;

        public string LockRegionEndPoint { get; set; } = null!;
        public string LockAccessKeyId { get; set; } = null!;
        public string LockAccessKeySecret { get; set; } = null!;
        public string LockTableName { get; set; } = null!;
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }
        public string CurlLocation { get; set; } = null!;
    }
}
