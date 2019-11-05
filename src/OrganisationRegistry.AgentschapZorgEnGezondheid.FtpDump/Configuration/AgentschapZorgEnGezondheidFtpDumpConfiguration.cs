namespace OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump.Configuration
{
    using System;
    using Newtonsoft.Json;
    using Infrastructure.Infrastructure.Json;

    public class AgentschapZorgEnGezondheidFtpDumpConfiguration
    {
        public static string Section = "AgentschapZorgEnGezondheidFtpDump";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string XmlLocation { get; set; }

        public string Host { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string FtpPath { get; set; }

        public AgentschapZorgEnGezondheidFtpDumpConfiguration Obfuscate()
        {
            return new AgentschapZorgEnGezondheidFtpDumpConfiguration
            {
                XmlLocation = XmlLocation,
                Host = Host,
                User = User,
                Pass = new string('*', 12),
                FtpPath = FtpPath
            };
        }
    }
}
