namespace OrganisationRegistry.KboMutations
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Configuration;
    using CsvHelper;
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
        private readonly CsvHelper.Configuration.CsvConfiguration _csvFileConfiguration;
        private readonly FtpUriBuilder _sourcePath;
        private readonly FtpUriBuilder _destinationPath;

        public KboMutationsFetcher(
            ILogger<KboMutationsFetcher> logger,
            IOptions<KboMutationsConfiguration> kboMutationsConfiguration,
            IFtpsClient curlFtpsClient)
        {
            _logger = logger;
            _curlFtpsClient = curlFtpsClient;
            _kboMutationsConfiguration = kboMutationsConfiguration.Value;

            _csvFileConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false
            };
        }

        public IEnumerable<MutationsFile> GetKboMutationFiles()
        {
            _logger.LogInformation("Fetching mutation files from folder {SourcePath}.",
                _kboMutationsConfiguration.SourcePath);

            var mutationFiles = _curlFtpsClient.GetListing(_kboMutationsConfiguration.SourcePath)
                .Select(GetMutationFile)
                .Values()
                .OrderBy(item => item.FullName)
                .ToList();

            _logger.LogInformation("Found {NumberOfMutationFiles} mutation files to process.", mutationFiles.Count);

            return mutationFiles;
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
                if (!_curlFtpsClient.Download(stream, ftpsListItem.FullName))
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

        public void Archive(MutationsFile file)
        {
            _logger.LogInformation("Archiving {FileName} to {ArchivePath}", file.FullName,
                _kboMutationsConfiguration.CachePath);

            _curlFtpsClient.MoveFile(
                file.FullName,
                _kboMutationsConfiguration.CachePath);
        }
    }
}
