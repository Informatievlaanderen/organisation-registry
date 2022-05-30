namespace OrganisationRegistry.KboMutations.Ftps;

using System;
using System.Diagnostics;
using System.IO;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class CurlFtpsClient : IFtpsClient
{
    private readonly ILogger<CurlFtpsClient> _logger;
    private readonly KboMutationsConfiguration _kboMutationsConfiguration;

    public CurlFtpsClient(
        ILogger<CurlFtpsClient> logger,
        IOptions<KboMutationsConfiguration> kboMutationsConfiguration)
    {
        _logger = logger;
        _kboMutationsConfiguration = kboMutationsConfiguration.Value;
    }

    public string GetListing(string sourceDirectory)
    {
        _logger.LogInformation("Fetching mutation files from folder {SourcePath}", sourceDirectory);

        using (var process = new Process
               {
                   StartInfo = new ProcessStartInfo
                   {
                       FileName = _kboMutationsConfiguration.CurlLocation,
                       Arguments = $"--ftp-ssl " +
                                   $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                   $"--cert {_kboMutationsConfiguration.CertPath} " +
                                   $"--key {_kboMutationsConfiguration.KeyPath} --key-type {_kboMutationsConfiguration.KeyType} " +
                                   $"--cacert {_kboMutationsConfiguration.CaCertPath} " +
                                   $"{sourceDirectory} --fail --silent --show-error",
                       RedirectStandardOutput = true,
                       RedirectStandardError = true,
                       UseShellExecute = false,
                       CreateNoWindow = true
                   }
               })
        {
            process.Start();
            process.WaitForExit();
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
                throw new Exception($"Could not list files at {sourceDirectory}:\n{error}");

            return result;
        }
    }

    public bool Download(Stream stream, string sourceFilePath)
    {
        var fileName = Guid.NewGuid().ToString();
        using (var process = new Process
               {
                   StartInfo = new ProcessStartInfo
                   {
                       FileName = _kboMutationsConfiguration.CurlLocation,
                       Arguments = $"--ftp-ssl " +
                                   $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                   $"--cert {_kboMutationsConfiguration.CertPath} " +
                                   $"--key {_kboMutationsConfiguration.KeyPath} --key-type {_kboMutationsConfiguration.KeyType} " +
                                   $"--cacert {_kboMutationsConfiguration.CaCertPath} " +
                                   $"{sourceFilePath} " +
                                   $"-o {fileName} --fail --silent --show-error",
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
                _logger.LogError("Could not download file {FileFullName}:\n{Error}", sourceFilePath, error);
                return false;
            }

            var readAllBytes = File.ReadAllBytes(fileName);
            stream.Write(readAllBytes);
            File.Delete(fileName);

            return true;
        }
    }

    public void MoveFile(string baseUri, string sourceFilePath, string destinationFilePath)
    {
        using (var process = new Process
               {
                   StartInfo = new ProcessStartInfo
                   {
                       FileName = _kboMutationsConfiguration.CurlLocation,
                       Arguments = $"--ftp-ssl " +
                                   $"--user {_kboMutationsConfiguration.Username}:{_kboMutationsConfiguration.Password} " +
                                   $"--cert {_kboMutationsConfiguration.CertPath} " +
                                   $"--key {_kboMutationsConfiguration.KeyPath} --key-type {_kboMutationsConfiguration.KeyType} " +
                                   $"--cacert {_kboMutationsConfiguration.CaCertPath} " +
                                   $"{baseUri} " +
                                   $"-Q \"-RNFR {sourceFilePath}\" " +
                                   $"-Q \"-RNTO {destinationFilePath}\" --fail --silent --show-error",
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
                    sourceFilePath,
                    destinationFilePath,
                    error);
        }
    }
}