namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class CachingConfigurationSection
{
    public static string Name = "Caching";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public int? UserCacheSlidingExpirationInMinutes { get; set; }
}