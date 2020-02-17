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

        public string ConnectionString { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        public string LoggingIndexFormat { get; set; }
        public string LoggingTemplateName { get; set; }
        public string LoggingTypeName { get; set; }
        public int? LoggingNumberOfShards { get; set; }

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

        public int MaxRetryAttempts { get; set; }

        public ElasticSearchConfiguration Obfuscate()
        {
            return new ElasticSearchConfiguration
            {
                ConnectionString = ConnectionString,
                User = User,
                Pass = new string('*', 12),

                LoggingIndexFormat = LoggingIndexFormat,
                LoggingTemplateName = LoggingTemplateName,
                LoggingTypeName = LoggingTypeName,
                LoggingNumberOfShards = LoggingNumberOfShards,

                OrganisationsReadIndex = OrganisationsReadIndex,
                OrganisationsWriteIndex = OrganisationsWriteIndex,
                OrganisationType = OrganisationType,

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
