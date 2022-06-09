namespace OrganisationRegistry.UnitTests.ImportFileProcessor;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.HostedServices.ProcessImportedFiles;
using Autofac.Features.OwnedInstances;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Import;
using SqlServer;
using SqlServer.Import.Organisations;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Tests.Shared;
using Xunit;

public class GivenImportFileInQueue : IDisposable
{
    private readonly HostedServiceConfiguration _configuration;

    public GivenImportFileInQueue()
    {
        _configuration = new HostedServiceConfiguration(delayInSeconds: 0, enabled: true);
    }

    [Fact]
    public async Task WhenItIsProcessedSuccesfully_ThenTheStatusIsUpdatedAsProcessed()
    {
        var fixture = new Fixture();

        var context = new OrganisationRegistryContext(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    "import-test",
                    _ => { }).Options);

        var contextFactory = new ContextFactory(() => new Owned<OrganisationRegistryContext>(context, this));
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);

        var importFileResulterMock = new Mock<IImportFileParserAndValidator>();
        var outputRecord = OutputRecord.From(
            new DeserializedRecord
            {
                Name = Field.FromValue(ColumnNames.Name, "name1"),
                Parent = Field.FromValue(ColumnNames.Parent, "ovo000012"),
                Reference = Field.FromValue(ColumnNames.Reference, "ref1"),
            },
            Guid.NewGuid(),
            1);

        importFileResulterMock
            .Setup(
                x =>
                    x.ParseAndValidate(It.IsAny<ImportOrganisationsStatusListItem>(), context, dateTimeProviderStub))
            .Returns(() => ParseAndValidatorResult.ForRecords(new List<OutputRecord> { outputRecord }));

        const string importFileResourceName = "OrganisationRegistry.UnitTests.ImportFileProcessor.expected_input.csv";
        var importCsvFileData = GetType().Assembly.GetResourceString(importFileResourceName);

        var importId = Guid.NewGuid();
        context.Add(
            new ImportOrganisationsStatusListItem
            {
                Id = importId,
                FileContent = importCsvFileData,
                FileName = string.Empty,
                Status = ImportProcessStatus.Processing,
                UserId = "userId",
                UserName = "userName",
                UserFirstName = "userFirstName",
            });

        var organisationDetailItem = new OrganisationDetailItem
        {
            Id = fixture.Create<Guid>(),
            Name = outputRecord.Name,
            SourceId = importId,
            SourceType = OrganisationSource.CsvImport,
            SourceOrganisationIdentifier = outputRecord.Reference,
            OvoNumber = "ovo000013"
        };
        context.Add(organisationDetailItem);

        await context.SaveChangesAsync();

        await ImportFileProcessor.ProcessNextFile(
            contextFactory,
            dateTimeProviderStub,
            Mock.Of<ILogger>(),
            importFileResulterMock.Object,
            Mock.Of<ICommandSender>(),
            _configuration,
            CancellationToken.None);

        var importStatus = await context.ImportOrganisationsStatusList.SingleAsync(item => item.Id == importId);

        importStatus.LastProcessedAt.Should().BeSameDateAs(new DateTimeOffset(dateTimeProviderStub.Today));
        importStatus.Status.Should().Be(ImportProcessStatus.Processed);

        const string outputFileResourceName = "OrganisationRegistry.UnitTests.ImportFileProcessor.expected_output.csv";
        var expectedOutputCsv = GetType().Assembly.GetResourceString(outputFileResourceName);

        importStatus.OutputFileContent.Should().Be(expectedOutputCsv);
    }

    [Fact]
    public async Task WhenItIsProcessedWithIssues_ThenTheStatusIsUpdatedAsFailed()
    {
        var context = new OrganisationRegistryContext(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    "import-test",
                    _ => { }).Options);

        var contextFactory = new ContextFactory(() => new Owned<OrganisationRegistryContext>(context, this));
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);

        var importFileResulterMock = new Mock<IImportFileParserAndValidator>();
        importFileResulterMock
            .Setup(
                x => x.ParseAndValidate(It.IsAny<ImportOrganisationsStatusListItem>(), context, dateTimeProviderStub))
            .Returns(
                () => ParseAndValidatorResult.ForIssues(
                    new ValidationIssues().Add(new ValidationIssue(1, "validation error"))));

        var importId = Guid.NewGuid();
        context.Add(
            new ImportOrganisationsStatusListItem
            {
                Id = importId,
                FileContent = string.Empty,
                FileName = string.Empty,
                Status = ImportProcessStatus.Processing,
                UserId = "userId",
                UserName = "userName",
                UserFirstName = "userFirstName",
            });

        await context.SaveChangesAsync();

        await ImportFileProcessor.ProcessNextFile(
            contextFactory,
            dateTimeProviderStub,
            Mock.Of<ILogger>(),
            importFileResulterMock.Object,
            Mock.Of<ICommandSender>(),
            _configuration,
            CancellationToken.None);

        var importStatus = await context.ImportOrganisationsStatusList.SingleAsync(item => item.Id == importId);

        importStatus.LastProcessedAt.Should().BeSameDateAs(new DateTimeOffset(dateTimeProviderStub.Today));
        importStatus.Status.Should().Be(ImportProcessStatus.Failed);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
