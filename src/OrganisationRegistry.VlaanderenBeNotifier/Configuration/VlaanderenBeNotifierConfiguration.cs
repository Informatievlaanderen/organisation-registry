namespace OrganisationRegistry.VlaanderenBeNotifier.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class VlaanderenBeNotifierConfiguration
{
    public static string Section = "VlaanderenBeNotifier";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public string SendGridApiUri { get; set; } = null!;
    public string SendGridBearerToken { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string FromName { get; set; } = null!;
    public string OrganisationTo { get; set; } = null!;
    public string OrganisationUriTemplate { get; set; } = null!;
    public string BodyTo { get; set; } = null!;
    public string BodyUriTemplate { get; set; } = null!;
    public string BodyFormalFrameworkUriTemplate { get; set; } = null!;
    public Guid MepFormalFrameworkId { get; set; }

    public string LockRegionEndPoint { get; set; } = null!;
    public string LockAccessKeyId { get; set; } = null!;
    public string LockAccessKeySecret { get; set; } = null!;
    public string LockTableName { get; set; } = null!;
    public int LockLeasePeriodInMinutes { get; set; }
    public bool LockEnabled { get; set; }

    public VlaanderenBeNotifierConfiguration Obfuscate()
        => new()
        {
            SendGridApiUri = SendGridApiUri,
            SendGridBearerToken = new string('*', 12),
            FromAddress = FromAddress,
            FromName = FromName,
            OrganisationTo = OrganisationTo,
            OrganisationUriTemplate = OrganisationUriTemplate,
            BodyTo = BodyTo,
            BodyUriTemplate = BodyUriTemplate,
            BodyFormalFrameworkUriTemplate = BodyFormalFrameworkUriTemplate,
            MepFormalFrameworkId = MepFormalFrameworkId,
        };
}
