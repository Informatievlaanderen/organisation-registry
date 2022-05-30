namespace OrganisationRegistry.MagdaReRegistration.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class MagdaReRegistrationConfiguration
{
    public static string Section = "MagdaReRegistration";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public string OrganisationRegistryApiUri { get; set; }

    public MagdaReRegistrationConfiguration Obfuscate()
    {
        return new MagdaReRegistrationConfiguration();
    }
}