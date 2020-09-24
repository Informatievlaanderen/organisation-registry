namespace OrganisationRegistry.KboMutations
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Configuration;
    using CsvHelper;
    using CsvHelper.Configuration;
    using Ftps;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Optional;
    using Optional.Collections;

    public class KboMutationsFetcher : IKboMutationsFetcher
    {
        private readonly ILogger<KboMutationsFetcher> _logger;
        private readonly IFtpsClient _curlFtpsClient;
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;
        private readonly CsvConfiguration _csvFileConfiguration;
        private readonly FtpUriBuilder _baseUriBuilder;

        public KboMutationsFetcher(
            ILogger<KboMutationsFetcher> logger,
            IOptions<KboMutationsConfiguration> kboMutationsConfiguration,
            IFtpsClient curlFtpsClient)
        {
            _logger = logger;
            _curlFtpsClient = curlFtpsClient;
            _kboMutationsConfiguration = kboMutationsConfiguration.Value;

            _csvFileConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false
            };

            _baseUriBuilder = new FtpUriBuilder(_kboMutationsConfiguration.Host, _kboMutationsConfiguration.Port);
        }

        public IEnumerable<MutationsFile> GetKboMutationFiles()
        {
            _logger.LogInformation(
                "Fetching mutation files from folder {SourcePath}.",
                _kboMutationsConfiguration.SourcePath);

            var sourceDirectoryUri = _baseUriBuilder.AppendDir(_kboMutationsConfiguration.SourcePath);

            var curlListResult = _curlFtpsClient.GetListing(sourceDirectoryUri.ToString());
            var mutationFiles =
                FtpsListParser.Parse(sourceDirectoryUri, curlListResult)
                    .Select(GetMutationFile)
                    .Values()
                    .OrderBy(item => item.FullName)
                    .ToList();

            _logger.LogInformation(
                "Found {NumberOfMutationFiles} mutation files to process.",
                mutationFiles.Count);

            return mutationFiles;
        }

        public void Archive(MutationsFile file)
        {
            _logger.LogInformation(
                "Archiving {FileName} to {ArchivePath}",
                file.FullName,
                _kboMutationsConfiguration.CachePath);

            var sourceFullNameUri = _baseUriBuilder.WithPath(file.FullName);
            var destinationFullNameUri = _baseUriBuilder
                .AppendDir(_kboMutationsConfiguration.CachePath)
                .AppendFileName(sourceFullNameUri.FileName);

            _curlFtpsClient.MoveFile(_baseUriBuilder.ToString(),
                sourceFullNameUri.Path,
                destinationFullNameUri.Path);
        }

        private Option<MutationsFile> GetMutationFile(FtpsListItem ftpsListItem)
        {
            if (ftpsListItem.Size == 0L)
                return new MutationsFile
                {
                    FullName = ftpsListItem.FullName,
                    Name = ftpsListItem.Name,
                    KboMutations = new List<MutationsLine>()
                }.Some();

            using (var stream = new MemoryStream())
            {
                var fullNameUri = _baseUriBuilder.WithPath(ftpsListItem.FullName);
                if (!_curlFtpsClient.Download(stream, fullNameUri.ToString()))
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
                            FullName = ftpsListItem.FullName,
                            Name = ftpsListItem.Name,
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
    }
}
