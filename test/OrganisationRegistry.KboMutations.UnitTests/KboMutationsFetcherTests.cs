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
        private const string MutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutations.csv";
        private const string InvalidMutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutationsInvalid.csv";

        [Fact]
        public void ReadsAllFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpsClient = new Mock<IFtpsClient>();
                SetUpMock(ftpsClient,
                    new FtpsListItemStub("mutations.csv", mutationsCsv),
                    new FtpsListItemStub("mutations2.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpsClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .BeEquivalentTo("mutations.csv", "mutations2.csv");
            }
        }

        [Fact]
        public void ReturnsFilesAlphabetically()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("xyz.csv", mutationsCsv),
                    new FtpsListItemStub("abc.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .ContainInOrder("abc.csv", "xyz.csv");
            }
        }

        [Fact]
        public void AlsoReadsEmptyFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("mutations.csv", mutationsCsv),
                    new FtpsListItemStub("mutations2.csv", mutationsCsv, 0));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .BeEquivalentTo("mutations.csv", "mutations2.csv");
            }
        }

        [Fact]
        public void ReturnsEmptyListOfMutationsWhenFileIsEmpty()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("mutations.csv", mutationsCsv, 0));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles[0].KboMutations.Should().BeEquivalentTo(new List<MutationsLine>());
            }
        }

        [Fact]
        public void SkipsFilesThatFailToDownload()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("mutations.csv", mutationsCsv),
                    new FtpsListItemStub("mutations2.csv", mutationsCsv));

                ftpClient.Setup(x =>
                        x.Download(It.IsAny<Stream>(), "/source/mutations.csv"))
                    .Returns(false);

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .BeEquivalentTo("mutations2.csv");
            }
        }

        [Fact]
        public void SkipsFilesThatFailToBeParsed()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            using (var invalidMutationsCsv = assembly.GetManifestResourceStream(InvalidMutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("mutations.csv", invalidMutationsCsv),
                    new FtpsListItemStub("mutations2.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .BeEquivalentTo("mutations2.csv");
            }
        }

        [Fact]
        public void ReadsAllLines()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpsClient>();
                SetUpMock(ftpClient,
                    new FtpsListItemStub("mutations.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    new NullLogger<KboMutationsFetcher>(),
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    ftpClient.Object);

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles[0].KboMutations.Should().HaveCount(238);
            }
        }

        [Fact]
        public void ReturnsEmptyListWhenNoFiles()
        {
            var ftpClient = new Mock<IFtpsClient>();

            var kboFtpClient = new KboMutationsFetcher(
                new NullLogger<KboMutationsFetcher>(),
                new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                ftpClient.Object);

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles.Should().BeEquivalentTo(new List<MutationsFile>());
        }

        private static void SetUpMock(Mock<IFtpsClient> ftpClient, params FtpsListItemStub[] files)
        {
            const string sourcePath = "/source";

            ftpClient
                .Setup(x => x.GetListing(It.IsAny<string>()))
                .Returns(() =>
                    files.Select(x =>
                        new FtpsListItem(
                            x.Name,
                            $"{sourcePath}/{x.Name}",
                            sourcePath,
                            x.Size.ToString())).ToArray());

            foreach (var file in files)
            {
                var fullName = $"{sourcePath}/{file.Name}";
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
