﻿﻿namespace OrganisationRegistry.KboMutations.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Api.Status;
    using Configuration;
    using FluentAssertions;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class RunnerTests
    {
        private readonly IEventPublisher _publisher;
        private readonly ILogger<Runner> _logger;

        public RunnerTests()
        {
            _publisher = Mock.Of<IEventPublisher>();
            _logger = Mock.Of<ILogger<Runner>>();
        }

        [Fact]
        public void ReturnsFalseWhenNotEnabled()
        {
            var togglesConfiguration = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { KboMutationsAvailable = false });

            var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration { });

            var runner =
                new Runner(_logger,
                    togglesConfiguration,
                    kboMutationsConfiguration,
                    Mock.Of<IKboMutationsFetcher>(),
                    Mock.Of<IKboMutationsPersister>(),
                    Mock.Of<IExternalIpFetcher>());

            runner.Run().Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueWhenNoMutationFiles()
        {
            var togglesConfiguration = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { KboMutationsAvailable = true });

            var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration { });

            var runner =
                new Runner(_logger,
                    togglesConfiguration,
                    kboMutationsConfiguration,
                    Mock.Of<IKboMutationsFetcher>(),
                    Mock.Of<IKboMutationsPersister>(),
                    Mock.Of<IExternalIpFetcher>());

            runner.Run().Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrueWhenNoMutationsInFiles()
        {
            var togglesConfiguration = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { KboMutationsAvailable = true });

            var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration { });

            var kboFtpClientMock = new Mock<IKboMutationsFetcher>();
            kboFtpClientMock
                .Setup(ftpClient => ftpClient.GetKboMutationFiles())
                .Returns(new List<MutationsFile>
                {
                    new MutationsFile {FullName = "filename.csv", KboMutations = new List<MutationsLine>()}
                });

            var runner =
                new Runner(_logger,
                togglesConfiguration,
                kboMutationsConfiguration,
                kboFtpClientMock.Object,
                Mock.Of<IKboMutationsPersister>(),
                Mock.Of<IExternalIpFetcher>());

            runner.Run().Should().BeTrue();
        }


        [Fact]
        public void ArchivesEachFileAfterProcessing()
        {
            var togglesConfiguration = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { KboMutationsAvailable = true });

            var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration { });

            var kboFtpClientMock = new Mock<IKboMutationsFetcher>();
            var mutationsFiles = new List<MutationsFile>
            {
                new MutationsFile {FullName = "filename.csv", KboMutations = new List<MutationsLine>()},
                new MutationsFile {FullName = "filename2.csv", KboMutations = new List<MutationsLine>()}
            };
            kboFtpClientMock
                .Setup(ftpClient => ftpClient.GetKboMutationFiles())
                .Returns(mutationsFiles);

            var runner =
                new Runner(_logger,
                    togglesConfiguration,
                    kboMutationsConfiguration,
                    kboFtpClientMock.Object,
                    Mock.Of<IKboMutationsPersister>(),
                    Mock.Of<IExternalIpFetcher>());

            runner.Run();

            mutationsFiles.ForEach(file =>
                kboFtpClientMock.Verify(ftpClient => ftpClient.Archive(file), Times.Once));
        }

        [Fact]
        public void PersistsEachMutationFile()
        {
            var togglesConfiguration = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { KboMutationsAvailable = true });

            var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration { });

            var kboFtpClientMock = new Mock<IKboMutationsFetcher>();

            var mutationsLines1 = new List<MutationsLine>
            {
                new MutationsLine
                    {DatumModificatie = DateTime.Now, MaatschappelijkeNaam = "KBC", Ondernemingsnummer = "0918128212"}
            };

            var mutationsLines2 = new List<MutationsLine>
            {
                new MutationsLine
                    {DatumModificatie = DateTime.Now, MaatschappelijkeNaam = "KBC", Ondernemingsnummer = "0918128212"}
            };

            kboFtpClientMock
                .Setup(ftpClient => ftpClient.GetKboMutationFiles())
                .Returns(new List<MutationsFile>
                {
                    new MutationsFile {FullName = "filename.csv", KboMutations = mutationsLines1},
                    new MutationsFile {FullName = "filename2.csv", KboMutations = mutationsLines2}
                });

            var kboMutationsPersisterMock = new Mock<IKboMutationsPersister>();

            var runner =
                new Runner(_logger,
                    togglesConfiguration,
                    kboMutationsConfiguration,
                    kboFtpClientMock.Object,
                    kboMutationsPersisterMock.Object,
                    Mock.Of<IExternalIpFetcher>());

            runner.Run();

            kboMutationsPersisterMock.Verify(persister => persister.Persist("filename.csv", mutationsLines1), Times.Once);
            kboMutationsPersisterMock.Verify(persister => persister.Persist("filename2.csv", mutationsLines2), Times.Once);
        }
    }
}
