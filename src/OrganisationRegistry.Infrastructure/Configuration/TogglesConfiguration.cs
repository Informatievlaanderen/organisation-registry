namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class TogglesConfiguration
    {
        public static string Section = "Toggles";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        // Global Setting
        public bool ApplicationAvailable { get; set; }

        // Individual Components
        public bool ApiAvailable { get; set; }
        public bool ElasticSearchProjectionsAvailable { get; set; }
        public bool VlaanderenBeNotifierAvailable { get; set; }
        public bool DelegationsRunnerAvailable { get; set; }
        public bool AgentschapZorgEnGezondheidFtpDumpAvailable { get; set; }

        // Vlaanderen.be Notifier
        public bool SendVlaanderenBeNotifierMails { get; set; }

        //  Reporting
        public bool ReportingRunnerAvailable { get; set; }

        public bool KboMutationsAvailable { get; set; }

        public TogglesConfiguration() { }
    }
}
