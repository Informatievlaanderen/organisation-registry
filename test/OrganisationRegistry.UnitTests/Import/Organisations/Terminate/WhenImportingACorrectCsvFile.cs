namespace OrganisationRegistry.UnitTests.Import.Organisations.Terminate;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Api.HostedServices.ProcessImportedFiles.StopOrganisations;
using Api.HostedServices.ProcessImportedFiles.Processor;
using AutoFixture.Kernel;
using OrganisationRegistry.Api.Import.Organisations;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Organisation;
using Tests.Shared;
using Xunit;
using OrganisationRegistry.Infrastructure.Authorization;

public class WhenImportingACorrectCsvFile
{
    private readonly OrganisationRegistryContext _context;

    public WhenImportingACorrectCsvFile()
    {
        _context = new OrganisationRegistryContext(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    "import-test-" + Guid.NewGuid(),
                    _ => { })
                .Options);
    }

    private async Task<ProcessImportedFileResult> Process(string csvToParse)
    {
        var fixture = new Fixture();
        var sourceId = fixture.Create<Guid>();

        await _context.OrganisationDetail.AddRangeAsync(
            new List<OrganisationDetailItem>
            {
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003000", "name1"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003001", "name2"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003002", "name3"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003003", "name4"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003004", "name5"),
            });

        await _context.SaveChangesAsync();

        return await new ImportedFileProcessor(
                _context,
                Mock.Of<ICommandSender>())
            .Process(
                new ImportOrganisationsStatusListItem
                {
                    Id = sourceId,
                    FileContent = csvToParse,
                    FileName = fixture.Create<string>(),
                    Status = ImportProcessStatus.Processing,
                    ImportFileType = ImportFileTypes.Stop,
                    UserName = fixture.Create<string>(),
                    UserFirstName = fixture.Create<string>(),
                    UserId = fixture.Create<string>(),
                    UserRoles = string.Join(separator: '|',Role.AlgemeenBeheerder),
                    UploadedAt = fixture.Create<DateTime>(),
                },
                CancellationToken.None);
    }

    [Theory]
    [InlineData("input_1", "output_1")]
    public async Task GivenACorrectInputFile_ThenItReturnsASuccessOutputFile(string inputFile, string outputFile)
    {
        var result = await Process(GetType().GetAssociatedResourceCsv(inputFile));

        result.OutputFileContent.Should().Be(GetType().GetAssociatedResourceCsv(outputFile));
    }

    private static OrganisationDetailItem CreateOrganisationDetailItem(ISpecimenBuilder fixture, Guid sourceId, string ovoNumber, string name)
        => new()
        {
            Id = fixture.Create<Guid>(),
            Name = name,
            OvoNumber = ovoNumber,
            SourceId = sourceId,
            SourceType = OrganisationSource.CsvImport,
            SourceOrganisationIdentifier = "ref1",
        };
}
