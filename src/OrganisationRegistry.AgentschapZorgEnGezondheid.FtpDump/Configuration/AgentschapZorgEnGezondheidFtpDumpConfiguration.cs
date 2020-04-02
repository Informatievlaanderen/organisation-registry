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

        public string LockRegionEndPoint { get; set; }
        public string LockAccessKeyId { get; set; }
        public string LockAccessKeySecret { get; set; }
        public string LockTableName { get; set; }
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }

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
