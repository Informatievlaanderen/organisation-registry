namespace OrganisationRegistry.KboMutations.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using FluentAssertions;
    using Ftps;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class KboMutationsFetcherTests: IDisposable
    {
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;
        private readonly FtpUriBuilder _baseUriBuilder;
        private const string MutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutations.csv";
        private const string InvalidMutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutationsInvalid.csv";

        public KboMutationsFetcherTests()
        {
            _kboMutationsConfiguration = new KboMutationsConfiguration
            {
                SourcePath = "/source",
                CachePath = "/cache"
            };
            _baseUriBuilder = new FtpUriBuilder(_kboMutationsConfiguration.Host, _kboMutationsConfiguration.Port);
        }

        [Fact]
        public void ReadsAllFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpsClient = new Mock<IFtpsClient>();
            SetUpMock(ftpsClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv),
                new FtpsListItemStub("mutations2.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpsClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles
                .Select(file => file.Name)
                .Should()
                .BeEquivalentTo("mutations.csv", "mutations2.csv");
        }

        [Fact]
        public void ReturnsFilesAlphabetically()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("xyz.csv", mutationsCsv),
                new FtpsListItemStub("abc.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles
                .Select(file => file.Name)
                .Should()
                .ContainInOrder("abc.csv", "xyz.csv");
        }

        [Fact]
        public void AlsoReadsEmptyFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv),
                new FtpsListItemStub("mutations2.csv", mutationsCsv, 0));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles
                .Select(file => file.Name)
                .Should()
                .BeEquivalentTo("mutations.csv", "mutations2.csv");
        }

        [Fact]
        public void ReturnsEmptyListOfMutationsWhenFileIsEmpty()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv, 0));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles[0].KboMutations.Should().BeEquivalentTo(new List<MutationsLine>());
        }

        [Fact]
        public void SkipsFilesThatFailToDownload()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv),
                new FtpsListItemStub("mutations2.csv", mutationsCsv));

            ftpClient.Setup(x =>
                    x.Download(
                        It.IsAny<Stream>(),
                        _baseUriBuilder
                            .AppendDir(_kboMutationsConfiguration.SourcePath)
                            .AppendFileName("mutations.csv")
                            .ToString()))
                .Returns(false);

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles
                .Select(file => file.Name)
                .Should()
                .BeEquivalentTo("mutations2.csv");
        }

        [Fact]
        public void SkipsFilesThatFailToBeParsed()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            using var invalidMutationsCsv = assembly.GetManifestResourceStream(InvalidMutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", invalidMutationsCsv),
                new FtpsListItemStub("mutations2.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles
                .Select(file => file.Name)
                .Should()
                .BeEquivalentTo("mutations2.csv");
        }

        [Fact]
        public void ReadsAllLines()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles[0].KboMutations.Should().HaveCount(240);
        }

        [Fact]
        public void ReadsLinesWithStopzettingsDatum()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            var mutationsLine = kboMutationFiles[0].KboMutations.Single(line => line.Ondernemingsnummer == "0416936979");
            mutationsLine.StopzettingsDatum.Should().Be(new DateTime(2020, 04, 28));
            mutationsLine.StopzettingsCode.Should().Be("014");
            mutationsLine.StopzettingsReden.Should().Be("Sluiting van de vereffening");
            mutationsLine.StatusCode.Should().Be("ST");
        }

        [Fact]
        public void ReadsLinesWithoutStopzettingsDatum()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName)!;
            var ftpClient = new Mock<IFtpsClient>();
            SetUpMock(ftpClient,
                new FtpsListItemStub("mutations.csv", mutationsCsv));

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            var mutationsLine = kboMutationFiles[0].KboMutations.Single(line => line.Ondernemingsnummer == "0808766994");
            mutationsLine.StopzettingsDatum.Should().BeNull();
            mutationsLine.StopzettingsCode.Should().BeEmpty();
            mutationsLine.StopzettingsReden.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsEmptyListWhenNoFiles()
        {
            var ftpClient = new Mock<IFtpsClient>();

            SetUpMock(ftpClient);

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(_kboMutationsConfiguration),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles.Should().BeEquivalentTo(new List<MutationsFile>());
        }

        private void SetUpMock(Mock<IFtpsClient> ftpClient, params FtpsListItemStub[] files)
        {
            var sourcePath = _kboMutationsConfiguration.SourcePath;

            ftpClient
                .Setup(x => x.GetListing(It.IsAny<string>()))
                .Returns(() =>
                    string.Join(
                        "\n",
                        files.Select(x => $"-rw-rw-r--   1 user user {x.Size.ToString(),8} Jun 13 17:49 {x.Name}")));

            foreach (var file in files)
            {
                var fullName = _baseUriBuilder.AppendDir(sourcePath).AppendFileName(file.Name).ToString();
                ftpClient.Setup(x =>
                        x.Download(It.IsAny<Stream>(), fullName))
                    .Callback<Stream, string>((stream, _) => file.Stream.CopyTo(stream))
                    .Returns(true);
            }
        }

        class FtpsListItemStub
        {
            public FtpsListItemStub(string name, Stream stream, long size = 1)
            {
                Name = name;
                Stream = stream;
                Size = size;
            }

            public string Name { get; }
            public Stream Stream { get; }
            public long Size { get; }
        }

        public void Dispose()
        {
        }
    }
}
