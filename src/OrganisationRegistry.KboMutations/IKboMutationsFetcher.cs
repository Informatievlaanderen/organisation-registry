namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Autofac.Features.OwnedInstances;
    using Configuration;
    using CsvHelper;
    using FluentFTP;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Optional;
    using Optional.Collections;

    public interface IKboMutationsFetcher
    {
        IEnumerable<MutationsFile> GetKboMutationFiles();
        void Archive(MutationsFile file);
    }

    public class KboMutationsFetcher : IKboMutationsFetcher
    {
        private readonly ILogger<KboMutationsFetcher> _logger;
        private readonly Func<Owned<IFtpClient>> _ftpClientFactory;
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;
        private readonly CsvHelper.Configuration.Configuration _csvFileConfiguration;

        public KboMutationsFetcher(ILogger<KboMutationsFetcher> logger,
            IOptions<KboMutationsConfiguration> kboMutationsConfiguration,
            Func<Owned<IFtpClient>> ftpClientFactory)
        {
            _logger = logger;
            _ftpClientFactory = ftpClientFactory;
            _kboMutationsConfiguration = kboMutationsConfiguration.Value;

            _csvFileConfiguration = new CsvHelper.Configuration.Configuration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false
            };

        }

        public IEnumerable<MutationsFile> GetKboMutationFiles()
        {
            var mutationFiles = new List<MutationsFile>();
            using (var ftpClient = _ftpClientFactory().Value)
            {
                ftpClient.Connect();

                mutationFiles.AddRange(
                    ftpClient.GetListing(_kboMutationsConfiguration.SourcePath)
                        .OrderBy(item => item.FullName)
                        .Select(ftpListItem => GetMutationFile(ftpListItem, ftpClient))
                        .Values());

                ftpClient.Disconnect();
            }

            return mutationFiles;
        }

        private Option<MutationsFile> GetMutationFile(FtpListItem ftpListItem, IFtpClient ftpClient)
        {
            if (ftpListItem.Size == 0L)
                return new MutationsFile
                {
                    FullName = ftpListItem.FullName,
                    Name = ftpListItem.Name,
                    KboMutations = new List<MutationsLine>()
                }.Some();

            using (var stream = new MemoryStream())
            {
                if (!ftpClient.Download(stream, ftpListItem.FullName, 0))
                    return Option.None<MutationsFile>();

                stream.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(stream))
                using (var reader = new CsvReader(streamReader, _csvFileConfiguration))
                {
                    try
                    {
                        var mutationsLines = reader.GetRecords<MutationsLine>();
                        return new MutationsFile
                        {
                            FullName = ftpListItem.FullName,
                            Name = ftpListItem.Name,
                            KboMutations = new List<MutationsLine>(mutationsLines)
                        }.Some();
                    }
                    catch
                    {
                        return Option.None<MutationsFile>();
                    }
                }
            }
        }

        public void Archive(MutationsFile file)
        {
            using (var ftpClient = CreateFtpClient())
            {
                ftpClient.Connect();

                ftpClient.MoveFile(file.FullName, _kboMutationsConfiguration.CachePath.TrimEnd('/') + "/" + file.Name);

                ftpClient.Disconnect();
            }
        }

        private FtpClient CreateFtpClient()
        {
            return new FtpClient
            {
                Host = _kboMutationsConfiguration.Host,
                Port = _kboMutationsConfiguration.Port,
                Credentials = new NetworkCredential(
                    _kboMutationsConfiguration.Username,
                    _kboMutationsConfiguration.Password),
            };
        }
    }
}
