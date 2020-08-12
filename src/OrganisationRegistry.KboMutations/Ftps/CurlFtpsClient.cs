namespace OrganisationRegistry.KboMutations.Ftps
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CurlFtpsClient : IFtpsClient
    {
        private readonly ILogger<CurlFtpsClient> _logger;
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;
        private readonly FtpUriBuilder _baseUriBuilder;

        public CurlFtpsClient(
            ILogger<CurlFtpsClient> logger,
            IOptions<KboMutationsConfiguration> kboMutationsConfiguration)
        {
            _logger = logger;
            _kboMutationsConfiguration = kboMutationsConfiguration.Value;
            _baseUriBuilder = new FtpUriBuilder(_kboMutationsConfiguration.Host, _kboMutationsConfiguration.Port);
        }

        public IEnumerable<FtpsListItem> GetListing(string sourceDirectory)
        {
            _logger.LogInformation("Fetching mutation files from folder {SourcePath}.", sourceDirectory);

            var sourceDirectoryUriBuilder = _baseUriBuilder.AppendDir(sourceDirectory);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _kboMutationsConfiguration.CurlLocation,
                    Arguments = $"--ftp-ssl " +
                                $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                $"{sourceDirectoryUriBuilder}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var ftpsListItems = FtpsListParser.Parse(sourceDirectoryUriBuilder, result).ToList();
            _logger.LogInformation("Found {NumberOfMutationFiles} mutation files to process.", ftpsListItems.Count);

            return ftpsListItems;
        }

        public bool Download(Stream stream, string fullName)
        {
            var fullNameUriBuilder = _baseUriBuilder.WithPath(fullName);
            var fileName = fullNameUriBuilder.FileName;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _kboMutationsConfiguration.CurlLocation,
                    Arguments = $"--ftp-ssl " +
                                $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                $"{fullNameUriBuilder} " +
                                $"-o {fileName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();

            var readAllBytes = File.ReadAllBytes(fileName);
            stream.Write(readAllBytes);

            return process.ExitCode == 0;
        }

        public void MoveFile(string sourceFileFullName, string destinationDirectory)
        {
            var sourceFullNameUriBuilder = _baseUriBuilder.WithPath(sourceFileFullName);
            var destinationFullNameUriBuilder = _baseUriBuilder
                .AppendDir(destinationDirectory)
                .AppendFileName(sourceFullNameUriBuilder.FileName);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _kboMutationsConfiguration.CurlLocation,
                    Arguments = $"--ftp-ssl " +
                                $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                $"{_baseUriBuilder} " +
                                $"-Q \"-RNFR {sourceFullNameUriBuilder.Path}\" " +
                                $"-Q \"-RNTO {destinationFullNameUriBuilder.Path}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
