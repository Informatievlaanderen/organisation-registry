namespace OrganisationRegistry.ElasticSearch.Configuration
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal class TimestampConverter : IsoDateTimeConverter
    {
        public TimestampConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        }
    }

    // Scoped as SingleInstance()
    public class ElasticSearchConfiguration
    {
        public static string Section = "ElasticSearch";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string WriteConnectionString { get; set; }
        public string ReadConnectionString { get; set; }
        public string WriteUser { get; set; }
        public string ReadUser { get; set; }
        public string WritePass { get; set; }
        public string ReadPass { get; set; }

        public string OrganisationsReadIndex { get; set; }
        public string OrganisationsWriteIndex { get; set; }
        public string OrganisationType { get; set; }

        public string PeopleReadIndex { get; set; }
        public string PeopleWriteIndex { get; set; }
        public string PersonType { get; set; }

        public string BodyReadIndex { get; set; }
        public string BodyWriteIndex { get; set; }
        public string BodyType { get; set; }

        public string LockRegionEndPoint { get; set; }
        public string LockAccessKeyId { get; set; }
        public string LockAccessKeySecret { get; set; }
        public string LockTableName { get; set; }
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }

        public int MaxRetryAttempts { get; set; }
        public int BatchSize { get; set; }
        public int NumberOfShards { get; set; }
        public int NumberOfReplicas { get; set; }

        public ElasticSearchConfiguration Obfuscate()
        {
            return new ElasticSearchConfiguration
            {
                WriteConnectionString = WriteConnectionString,
                ReadConnectionString = ReadConnectionString,
                WriteUser = WriteUser,
                ReadUser = ReadUser,
                WritePass = new string('*', 12),
                ReadPass = new string('*', 12),

                OrganisationsReadIndex = OrganisationsReadIndex,
                OrganisationsWriteIndex = OrganisationsWriteIndex,
                OrganisationType = OrganisationType,

                NumberOfShards = NumberOfShards,
                NumberOfReplicas = NumberOfReplicas,

                PeopleReadIndex = PeopleReadIndex,
                PeopleWriteIndex = PeopleWriteIndex,
                PersonType = PersonType,

                BodyReadIndex = BodyReadIndex,
                BodyWriteIndex = BodyWriteIndex,
                BodyType = BodyType,
            };
        }
    }
}
