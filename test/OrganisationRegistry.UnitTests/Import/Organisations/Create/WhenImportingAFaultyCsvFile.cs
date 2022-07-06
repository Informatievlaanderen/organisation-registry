namespace OrganisationRegistry.UnitTests.Import.Organisations.Create;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Processor;
using OrganisationRegistry.Api.Import.Organisations;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Organisation;
using SqlServer.Infrastructure;
using Tests.Shared;
using Xunit;

public class WhenImportingAFaultyCsvFile
{
    private readonly OrganisationRegistryContext _context;

    public WhenImportingAFaultyCsvFile()
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
        var today = DateOnly.FromDateTime(fixture.Create<DateTime>());
        var yesterday = today.AddDays(-1).ToDateTime(new TimeOnly());

        var parentId = fixture.Create<Guid>();

        await _context.OrganisationList.AddRangeAsync(
            new List<OrganisationListItem>
            {
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000012" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000013" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000014" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000015" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000016" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000017" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000018" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000019" },
                new() { Name = fixture.Create<string>(), OrganisationId = parentId, OvoNumber = "OVO002949" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO001828", ValidTo = yesterday },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO002950" },
                new() { OvoNumber = "OVO000123", ParentOrganisationId = parentId, Name = "afdeling hr" },
            });

        await _context.SaveChangesAsync();

        var statusItem = new ImportOrganisationsStatusListItem
        {
            Id = sourceId,
            FileContent = csvToParse,
            FileName = fixture.Create<string>(),
            Status = ImportProcessStatus.Processing,
            ImportFileType = ImportFileTypes.Create,
            UserName = fixture.Create<string>(),
            UserFirstName = fixture.Create<string>(),
            UserId = fixture.Create<string>(),
            UploadedAt = fixture.Create<DateTime>(),
        };
        return await new ImportedFileProcessor(
                _context,
                new DateTimeProviderStub(today.ToDateTime(new TimeOnly())),
                Mock.Of<ICommandSender>())
            .Process(
                statusItem,
                CancellationToken.None);
    }

    [Theory]
    [InlineData("input_1", "output_1")]
    [InlineData("input_2", "output_2")]
    public async Task ItReturnsAnErrorOutputFile(string inputFile, string outputFile)
    {
        var importCsv = GetType().GetAssociatedResourceCsv(inputFile);
        var outputCsv = GetType().GetAssociatedResourceCsv(outputFile);

        var result = await Process(importCsv);

        var resultFile = result.OutputFileContent.ReplaceLineEndings(Environment.NewLine);
        resultFile.Should().Be(outputCsv.ReplaceLineEndings(Environment.NewLine));
    }
}
