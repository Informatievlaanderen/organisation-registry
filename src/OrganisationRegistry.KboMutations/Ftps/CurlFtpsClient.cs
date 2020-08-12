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
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _kboMutationsConfiguration.CurlLocation,
                    Arguments = $"--ftp-ssl " +
                                $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                $"{sourceDirectoryUriBuilder}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            })
            {
                process.Start();
                process.WaitForExit();
                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                    _logger.LogError("Could not list files at {Source}:\n{Error}",
                        sourceDirectoryUriBuilder,
                        error);

                var ftpsListItems = FtpsListParser.Parse(sourceDirectoryUriBuilder, result).ToList();
                _logger.LogInformation("Found {NumberOfMutationFiles} mutation files to process.", ftpsListItems.Count);

                return ftpsListItems;
            }
        }

        public bool Download(Stream stream, string fullName)
        {
            var fullNameUriBuilder = _baseUriBuilder.WithPath(fullName);
            var fileName = fullNameUriBuilder.FileName;
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _kboMutationsConfiguration.CurlLocation,
                    Arguments = $"--ftp-ssl " +
                                $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                $"{fullNameUriBuilder} " +
                                $"-o {fileName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            })
            {
                process.Start();
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    _logger.LogError("Could not download file {FileFullName}:\n{Error}", fullName, error);
                    return false;
                }

                var readAllBytes = File.ReadAllBytes(fileName);
                stream.Write(readAllBytes);

                return true;
            }
        }

        public void MoveFile(string sourceFileFullName, string destinationDirectory)
        {
            var sourceFullNameUriBuilder = _baseUriBuilder.WithPath(sourceFileFullName);
            var destinationFullNameUriBuilder = _baseUriBuilder
                .AppendDir(destinationDirectory)
                .AppendFileName(sourceFullNameUriBuilder.FileName);

            using (var process = new Process
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
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            })
            {
                process.Start();
                process.WaitForExit();

                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                    _logger.LogError("Could not move file {Source} to {Destination}:\n{Error}",
                        sourceFullNameUriBuilder.Path,
                        destinationFullNameUriBuilder.Path,
                        error);
            }
        }
    }
}
