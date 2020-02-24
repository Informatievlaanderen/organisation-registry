namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Infrastructure.Json;
    using Newtonsoft.Json;

    public class TogglesConfiguration
    {
        public static string Section = "Toggles";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        // Global Setting
        public bool ApplicationAvailable { get; set; }
        public bool LogToElasticSearch { get; set; }

        // Individual Components
        public bool ApiAvailable { get; set; }
        public bool ElasticSearchProjectionsAvailable { get; set; }
        public bool VlaanderenBeNotifierAvailable { get; set; }
        public bool DelegationsRunnerAvailable { get; set; }
        public bool ElasticSearchJanitorAvailable { get; set; }
        public bool AgentschapZorgEnGezondheidFtpDumpAvailable { get; set; }

        // Vlaanderen.be Notifier
        public bool SendVlaanderenBeNotifierMails { get; set; }

        //  Reporting
        public bool ReportingRunnerAvailable { get; set; }
        public bool EnableReporting { get; set; }
        public bool EnableVademecumParticipationReporting { get; set; }
        public bool EnableFormalFrameworkBodiesReporting { get; set; }

        // Monitoring
        public bool EnableMonitoring { get; set; }

        // Misc
        public bool EnableOrganisationRelations { get; set; }
        public bool EnableOrganisationOpeningHours { get; set; }

        public bool KboMutationsAvailable { get; set; }

        public TogglesConfiguration() { }
    }
}
