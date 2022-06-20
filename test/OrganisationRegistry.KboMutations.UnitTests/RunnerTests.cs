 namespace OrganisationRegistry.KboMutations.UnitTests;

 using System;
 using System.Collections.Generic;
 using Configuration;
 using FluentAssertions;
 using Infrastructure;
 using Infrastructure.Configuration;
 using Infrastructure.Events;
 using Microsoft.EntityFrameworkCore;
 using Microsoft.Extensions.Logging;
 using Microsoft.Extensions.Options;
 using Moq;
 using SqlServer;
 using SqlServer.Infrastructure;
 using Xunit;

 public class RunnerTests
 {
     private readonly ILogger<Runner> _logger;

     public RunnerTests()
     {
         Mock.Of<IEventPublisher>();
         _logger = Mock.Of<ILogger<Runner>>();
     }

     [Fact]
     public void ReturnsFalseWhenNotEnabled()
     {
         var togglesConfiguration = new OptionsWrapper<TogglesConfigurationSection>(new TogglesConfigurationSection { KboMutationsAvailable = false });

         var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration());

         var runner =
             new Runner(_logger,
                 togglesConfiguration,
                 kboMutationsConfiguration,
                 Mock.Of<IKboMutationsFetcher>(),
                 Mock.Of<IKboMutationsPersister>(),
                 Mock.Of<IExternalIpFetcher>(),
                 Mock.Of<IContextFactory>());

         runner.Run().Should().BeFalse();
     }

     [Fact]
     public void ReturnsTrueWhenNoMutationFiles()
     {
         var togglesConfiguration = new OptionsWrapper<TogglesConfigurationSection>(new TogglesConfigurationSection { KboMutationsAvailable = true });

         var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration());

         var runner =
             new Runner(_logger,
                 togglesConfiguration,
                 kboMutationsConfiguration,
                 Mock.Of<IKboMutationsFetcher>(),
                 Mock.Of<IKboMutationsPersister>(),
                 Mock.Of<IExternalIpFetcher>(),
                 Mock.Of<IContextFactory>());

         runner.Run().Should().BeTrue();
     }

     [Fact]
     public void ReturnsTrueWhenNoMutationsInFiles()
     {
         var togglesConfiguration = new OptionsWrapper<TogglesConfigurationSection>(new TogglesConfigurationSection { KboMutationsAvailable = true });

         var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration());

         var kboFtpClientMock = new Mock<IKboMutationsFetcher>();
         kboFtpClientMock
             .Setup(ftpClient => ftpClient.GetKboMutationFiles())
             .Returns(
                 new List<MutationsFile>
                 {
                     new("filename.csv", string.Empty, new List<MutationsLine>()),
                 });

         var runner =
             new Runner(_logger,
                 togglesConfiguration,
                 kboMutationsConfiguration,
                 kboFtpClientMock.Object,
                 Mock.Of<IKboMutationsPersister>(),
                 Mock.Of<IExternalIpFetcher>(),
                 Mock.Of<IContextFactory>());

         runner.Run().Should().BeTrue();
     }


     [Fact]
     public void ArchivesEachFileAfterProcessing()
     {
         var togglesConfiguration = new OptionsWrapper<TogglesConfigurationSection>(new TogglesConfigurationSection { KboMutationsAvailable = true });

         var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration());

         var kboFtpClientMock = new Mock<IKboMutationsFetcher>();
         var mutationsFiles = new List<MutationsFile>
         {
             new("filename.csv", string.Empty, new List<MutationsLine>()),
             new("filename2.csv", string.Empty, new List<MutationsLine>()),
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
                 Mock.Of<IExternalIpFetcher>(),
                 Mock.Of<IContextFactory>());

         runner.Run();

         mutationsFiles.ForEach(file =>
             kboFtpClientMock.Verify(ftpClient => ftpClient.Archive(file), Times.Once));
     }

     [Fact]
     public void PersistsEachMutationFile()
     {
         var togglesConfiguration = new OptionsWrapper<TogglesConfigurationSection>(new TogglesConfigurationSection { KboMutationsAvailable = true });

         var kboMutationsConfiguration = new OptionsWrapper<KboMutationsConfiguration>(new KboMutationsConfiguration());

         var kboFtpClientMock = new Mock<IKboMutationsFetcher>();

         var mutationsLines1 = new List<MutationsLine>
         {
             new MutationsLine
                 {DatumModificatie = DateTime.Now, MaatschappelijkeNaam = "KBC", Ondernemingsnummer = "0918128212"},
         };

         var mutationsLines2 = new List<MutationsLine>
         {
             new MutationsLine
                 {DatumModificatie = DateTime.Now, MaatschappelijkeNaam = "KBC", Ondernemingsnummer = "0918128212"},
         };

         kboFtpClientMock
             .Setup(ftpClient => ftpClient.GetKboMutationFiles())
             .Returns(new List<MutationsFile>
             {

                 new("filename.csv", string.Empty, mutationsLines1),
                 new("filename2.csv", string.Empty, mutationsLines2),
             });

         var kboMutationsPersisterMock = new Mock<IKboMutationsPersister>();

         var context = new OrganisationRegistryContext(new DbContextOptions<OrganisationRegistryContext>());
         var contextFactoryMock = new Mock<IContextFactory>();
         contextFactoryMock.Setup(factory => factory.Create()).Returns(context);
         var runner =
             new Runner(_logger,
                 togglesConfiguration,
                 kboMutationsConfiguration,
                 kboFtpClientMock.Object,
                 kboMutationsPersisterMock.Object,
                 Mock.Of<IExternalIpFetcher>(),
                 contextFactoryMock.Object);

         runner.Run();

         kboMutationsPersisterMock.Verify(persister => persister.Persist(context, "filename.csv", mutationsLines1), Times.Once);
         kboMutationsPersisterMock.Verify(persister => persister.Persist(context, "filename2.csv", mutationsLines2), Times.Once);
     }
 }
