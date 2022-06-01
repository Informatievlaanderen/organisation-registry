namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System;
using Api.HostedServices.ProcessImportedFiles;
using FluentAssertions;
using SqlServer.Import.Organisations;
using Tests.Shared;
using Xunit;

public class WhenImportingACsvFile
{
    private static string Parse(string csvToParse)
    {
        var importOrganisationStatusListItem = new ImportOrganisationsStatusListItem()
        {
            Id = Guid.NewGuid(),
            Status = ImportProcessStatus.Processing,
            FileContent = csvToParse,
            FileName = "dummy.csv",
            UploadedAt = DateTimeOffset.Now
        };

        return ImportFileParser.Parse(importOrganisationStatusListItem).serializedOutput;
    }

    [Fact]
    public void GivenItHasNoErrors_ItReturnsTheParsedFile()
    {
        var importCsv = GetType().Assembly.GetResourceString(
            "OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_GivenItHasNoErrors.csv");
        var outputCsv = GetType().Assembly.GetResourceString(
            "OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_GivenItHasNoErrors_ItReturnsTheParsedFile.csv");

        var result = Parse(importCsv);
        result.Should().Be(outputCsv);
    }

    [Fact]
    public void GivenItHasErrors_ItReturnsTheFileWithErrors()
    {
        var importCsv = GetType().Assembly.GetResourceString(
            "OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_GivenItHasErrors.csv");
        var outputCsv = GetType().Assembly.GetResourceString(
                "OrganisationRegistry.UnitTests.Import.Organisations.WhenImportingACsvFile_GivenItHasErrors_ItReturnsTheOriginalFileWithErrors.csv")
            .ReplaceLineEndings(OutputSerializer.NewLine);

        var result = Parse(importCsv);
        result.Should().Be(outputCsv);
    }
}
