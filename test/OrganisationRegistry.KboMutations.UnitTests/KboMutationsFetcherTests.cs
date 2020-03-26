namespace OrganisationRegistry.KboMutations.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Autofac.Features.OwnedInstances;
    using Configuration;
    using FluentAssertions;
    using FluentFTP;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class KboMutationsFetcherTests: IDisposable
    {
        private DateTime _dateTime;
        private const string MutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutations.csv";
        private const string InvalidMutationsResourceName = "OrganisationRegistry.KboMutations.UnitTests.mutationsInvalid.csv";

        [Fact]
        public void ReadsAllFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", mutationsCsv),
                    new FtpClientFile("mutations2.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

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
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("xyz.csv", mutationsCsv),
                    new FtpClientFile("abc.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles
                    .Select(file => file.Name)
                    .Should()
                    .BeEquivalentTo("abc.csv", "xyz.csv");
            }
        }

        [Fact]
        public void AlsoReadsEmptyFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var mutationsCsv = assembly.GetManifestResourceStream(MutationsResourceName))
            {
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", mutationsCsv),
                    new FtpClientFile("mutations2.csv", mutationsCsv, 0));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

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
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", mutationsCsv, 0));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

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
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", mutationsCsv),
                    new FtpClientFile("mutations2.csv", mutationsCsv));

                ftpClient.Setup(x =>
                        x.Download(It.IsAny<Stream>(), "mutations.csv", It.IsAny<long>(), It.IsAny<Action<FtpProgress>>()))
                    .Returns(false);

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

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
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", invalidMutationsCsv),
                    new FtpClientFile("mutations2.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

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
                var ftpClient = new Mock<IFtpClient>();
                SetUpMock(ftpClient,
                    new FtpClientFile("mutations.csv", mutationsCsv));

                var kboFtpClient = new KboMutationsFetcher(
                    null,
                    new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                    new FtpClientFactoryStub(ftpClient.Object));

                var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

                kboMutationFiles[0].KboMutations.Should().HaveCount(238);
            }
        }

        [Fact]
        public void ReturnsEmptyListWhenNoFiles()
        {
            var ftpClient = new Mock<IFtpClient>();

            var kboFtpClient = new KboMutationsFetcher(
                null,
                new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration()),
                new FtpClientFactoryStub(ftpClient.Object));

            var kboMutationFiles = kboFtpClient.GetKboMutationFiles().ToList();

            kboMutationFiles.Should().BeEquivalentTo(new List<MutationsFile>());
        }

        private void SetUpMock(Mock<IFtpClient> ftpClient, params FtpClientFile[] files)
        {
            ftpClient.Setup(x => x.GetListing(It.IsAny<string>()))
                .Returns(() => files.Select(x => new FtpListItem(x.Name, x.Name, x.Size, false, ref _dateTime){FullName = x.Name}).ToArray());

            foreach (var file in files)
            {
                ftpClient.Setup(x =>
                        x.Download(It.IsAny<Stream>(), file.Name, It.IsAny<long>(), It.IsAny<Action<FtpProgress>>()))
                    .Callback<Stream, string, long, Action<FtpProgress>>((stream, s, arg3, arg4) => file.Stream.CopyTo(stream))
                    .Returns(true);
            }
        }

        class FtpClientFile
        {
            public FtpClientFile(string name, Stream stream, long size = 1)
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

    public class FtpClientFactoryStub : IFtpClientFactory
    {
        private readonly IFtpClient _ftpClient;

        public FtpClientFactoryStub(IFtpClient ftpClient)
        {
            _ftpClient = ftpClient;
        }

        public IFtpClient CreateFtpClient(KboMutationsConfiguration kboMutationsConfiguration)
        {
            return _ftpClient;
        }
    }
}
