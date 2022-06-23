namespace OrganisationRegistry.UnitTests.Import.Organisations.Create;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Processor;
using Api.Import.Organisations;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.SqlServer.Import.Organisations;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Tests.Shared;
using Xunit;

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

        await _context.OrganisationList.AddRangeAsync(
            new List<OrganisationListItem>
            {
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000012" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000013" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000014" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000015" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000016" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000017" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000018" },
                new() { FormalFrameworkId = null, Name = fixture.Create<string>(), OvoNumber = "OVO000019" },
            }
        );

        await _context.OrganisationDetail.AddRangeAsync(
            new List<OrganisationDetailItem>
            {
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003000"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003001"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003002"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003003"),
                CreateOrganisationDetailItem(fixture, sourceId, "OVO003004"),
            });

        await _context.SaveChangesAsync();

        return await new ImportedFileProcessor(
                _context,
                new DateTimeProviderStub(fixture.Create<DateTime>()),
                Mock.Of<ICommandSender>())
            .Process(
                new ImportOrganisationsStatusListItem
                {
                    Id = sourceId,
                    FileContent = csvToParse,
                    FileName = fixture.Create<string>(),
                    Status = ImportProcessStatus.Processing,
                    ImportFileType = ImportFileTypes.Create,
                    UserName = fixture.Create<string>(),
                    UserFirstName = fixture.Create<string>(),
                    UserRoles = $"{Role.VlimpersBeheerder}",
                    UserId = fixture.Create<string>(),
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

    private static OrganisationDetailItem CreateOrganisationDetailItem(ISpecimenBuilder fixture, Guid sourceId, string ovoNumber)
        => new() { Id = fixture.Create<Guid>(), Name = fixture.Create<string>(), OvoNumber = ovoNumber, SourceId = sourceId, SourceType = OrganisationSource.CsvImport, SourceOrganisationIdentifier = "ref1", };
}
