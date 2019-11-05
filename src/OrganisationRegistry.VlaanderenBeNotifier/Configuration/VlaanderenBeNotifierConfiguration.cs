namespace OrganisationRegistry.VlaanderenBeNotifier.Configuration
{
    using System;
    using Infrastructure.Infrastructure.Json;
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
