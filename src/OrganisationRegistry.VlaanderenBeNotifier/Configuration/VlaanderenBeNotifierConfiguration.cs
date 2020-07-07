namespace OrganisationRegistry.VlaanderenBeNotifier.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class VlaanderenBeNotifierConfiguration
    {
        public static string Section = "VlaanderenBeNotifier";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string SendGridApiUri { get; set; }
        public string SendGridBearerToken { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string OrganisationTo { get; set; }
        public string OrganisationUriTemplate { get; set; }
        public string BodyTo { get; set; }
        public string BodyUriTemplate { get; set; }
        public string BodyFormalFrameworkUriTemplate { get; set; }
        public Guid MepFormalFrameworkId { get; set; }

        public string LockRegionEndPoint { get; set; }
        public string LockAccessKeyId { get; set; }
        public string LockAccessKeySecret { get; set; }
        public string LockTableName { get; set; }
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }

        public VlaanderenBeNotifierConfiguration Obfuscate()
        {
            return new VlaanderenBeNotifierConfiguration
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
                MepFormalFrameworkId = MepFormalFrameworkId
            };
        }
    }
}
