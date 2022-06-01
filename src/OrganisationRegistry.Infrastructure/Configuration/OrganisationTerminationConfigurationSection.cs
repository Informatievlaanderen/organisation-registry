namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class OrganisationTerminationConfigurationSection
{
    public static string Name = "OrganisationTermination";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created
        => DateTime.Now;

    public string OrganisationCapacityIdsToTerminateEndOfNextYear { get; set; } = null!;
    public string OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; set; } = null!;
    public string FormalFrameworkIdsToTerminateEndOfNextYear { get; set; } = null!;
}
