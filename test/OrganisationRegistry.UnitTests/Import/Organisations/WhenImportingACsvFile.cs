namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System;
using System.Collections.Generic;
using Api.HostedServices.ProcessImportedFiles;
using AutoFixture;
using FluentAssertions;
using SqlServer.Import.Organisations;
using SqlServer.Organisation;
using Tests.Shared;
using Xunit;

public class WhenImportingACsvFile
{
    private static string Parse(string csvToParse)
    {
        var fixture = new Fixture();
        var today = DateOnly.FromDateTime(fixture.Create<DateTime>());
        var yesterday = today.AddDays(-1).ToDateTime(new TimeOnly());

        var importOrganisationStatusListItem = new ImportOrganisationsStatusListItem()
        {
            Id = Guid.NewGuid(),
            Status = ImportProcessStatus.Processing,
            FileContent = csvToParse,
            FileName = "dummy.csv",
            UploadedAt = DateTimeOffset.Now
        };

        var parentId = fixture.Create<Guid>();

        var importCache = FakeImportCache.Create(
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

        var parseResult = ImportFileParser.Parse(importOrganisationStatusListItem);
        return ImportFileValidator.Validate(importCache, today, parseResult).serializedOutput;
    }

    [Theory]
    [InlineData("1_hasErrors_input.csv", "1_hasErrors_output.csv")]
    [InlineData("2_hasNoErrors_input.csv", "2_hasNoErrors_output.csv")]
    [InlineData("3_hasErrors_input.csv", "3_hasErrors_output.csv")]
    public void GivenAnInputFile_ThenItReturnsAnOutputFile(string inputFile, string outputFile)
    {
        var inputFileResourceName = $"OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_{inputFile}";
        var outputFileResourceName = $"OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_{outputFile}";

        var importCsv = GetType().Assembly.GetResourceString(inputFileResourceName);
        var outputCsv = GetType().Assembly.GetResourceString(outputFileResourceName);

        var result = Parse(importCsv).ReplaceLineEndings(Environment.NewLine);
        result.Should().Be(outputCsv.ReplaceLineEndings(Environment.NewLine));
    }
}
