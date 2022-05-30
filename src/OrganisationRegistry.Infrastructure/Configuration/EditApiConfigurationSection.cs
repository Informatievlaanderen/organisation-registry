namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class EditApiConfigurationSection
{
    public static string Name = "EditApi";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public Guid Orafin { get; set; }
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Authority { get; set; } = null!;
    public string IntrospectionEndpoint { get; set; } = null!;
}