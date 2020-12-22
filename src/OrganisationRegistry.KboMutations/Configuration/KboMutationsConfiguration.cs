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

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SourcePath { get; set; }
        public string CachePath { get; set; }

        public string CertPath { get; set; }
        public string CaCertPath { get; set; }
        public string KeyPath { get; set; }

        public string LockRegionEndPoint { get; set; }
        public string LockAccessKeyId { get; set; }
        public string LockAccessKeySecret { get; set; }
        public string LockTableName { get; set; }
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }
        public string CurlLocation { get; set; }
    }
}
