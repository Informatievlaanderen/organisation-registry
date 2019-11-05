namespace OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using Configuration;
    using FluentFTP;
    using Info;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Infrastructure.Configuration;

    public class Runner
    {
        public const string DelegationsRunnerProjectionName = "AgentschapZorgEnGezondheidFtpDump";

        private readonly ILogger<Runner> _logger;
        private readonly TogglesConfiguration _togglesConfiguration;
        private readonly AgentschapZorgEnGezondheidFtpDumpConfiguration _ftpDumpConfiguration;

        public Runner(
            ILogger<Runner> logger,
            IOptions<TogglesConfiguration> togglesConfigurationOptions,
            IOptions<AgentschapZorgEnGezondheidFtpDumpConfiguration> ftpDumpConfiguration)
        {
            _logger = logger;
            _ftpDumpConfiguration = ftpDumpConfiguration.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
        }

        public bool Run()
        {
            _logger.LogInformation(ProgramInformation.Build(_ftpDumpConfiguration));

            if (!_togglesConfiguration.AgentschapZorgEnGezondheidFtpDumpAvailable)
                return false;

            // Get XML via API, do a regular HTTP call as a consumer to avoid the entire DI setup
            var xmlDump = FetchXmlDump();

            // Upload to FTP
            UploadFile(xmlDump);

            return true;
        }

        private byte[] FetchXmlDump()
        {
            _logger.LogInformation("Calling {XmlDumpEndpoint} to get Agentschap Zorg en Gezondheid XML data.", _ftpDumpConfiguration.XmlLocation);
            var http = new HttpClient();

            return http.GetByteArrayAsync(_ftpDumpConfiguration.XmlLocation).GetAwaiter().GetResult();
        }

        private void UploadFile(byte[] xmlDump)
        {
            // var fileName = $"OrganisatieRegister-{DateTime.UtcNow.Date:yyyy-MM-dd}.xml";
            var fileName = $"OrganisatieRegister.xml";
            _logger.LogInformation("Uploading {XmlDumpFileName} to Agentschap Zorg en Gezondheid FTP server.", fileName);

            var path = Path.Combine(_ftpDumpConfiguration.FtpPath, fileName);
            var login = new NetworkCredential(_ftpDumpConfiguration.User, _ftpDumpConfiguration.Pass);
            var ftp = new FtpClient(_ftpDumpConfiguration.Host, login);
            ftp.Connect();

            if (ftp.FileExists(path))
                _logger.LogWarning("File {XmlDumpFile} already exists on the FTP server at {XmlDumpPath}!", fileName, path);

            ftp.RetryAttempts = 3;
            ftp.Upload(xmlDump, path);

            ftp.Disconnect();
        }
    }
}
