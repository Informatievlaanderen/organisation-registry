namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class OrganisationManagementConfigurationSection
{
    public static string Name = "OrganisationManagement";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public string Vlimpers { get; set; } = null!;
}
