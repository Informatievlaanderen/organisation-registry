namespace OrganisationRegistry.ElasticSearch.Configuration;

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

    public string WriteConnectionString { get; set; } = null!;
    public string ReadConnectionString { get; set; } = null!;
    public string WriteUser { get; set; } = null!;
    public string ReadUser { get; set; } = null!;
    public string WritePass { get; set; } = null!;
    public string ReadPass { get; set; } = null!;

    public string OrganisationsReadIndex { get; set; } = null!;
    public string OrganisationsWriteIndex { get; set; } = null!;
    public string OrganisationType { get; set; } = null!;

    public string PeopleReadIndex { get; set; } = null!;
    public string PeopleWriteIndex { get; set; } = null!;
    public string PersonType { get; set; } = null!;

    public string BodyReadIndex { get; set; } = null!;
    public string BodyWriteIndex { get; set; } = null!;
    public string BodyType { get; set; } = null!;

    public string LockRegionEndPoint { get; set; } = null!;
    public string LockAccessKeyId { get; set; } = null!;
    public string LockAccessKeySecret { get; set; } = null!;
    public string LockTableName { get; set; } = null!;
    public int LockLeasePeriodInMinutes { get; set; }
    public bool LockEnabled { get; set; }

    public int MaxRetryAttempts { get; set; }
    public int BatchSize { get; set; }
    public int NumberOfShards { get; set; }
    public int NumberOfReplicas { get; set; }

    public ElasticSearchConfiguration Obfuscate()
        => new()
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