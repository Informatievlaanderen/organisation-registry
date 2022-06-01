namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System;
using System.Collections.Generic;
using System.Linq;
using Api.HostedServices.ProcessImportedFiles;
using FluentAssertions;
using Xunit;

public class WhenValidatingTheCsvImportFileContent
{
    private static List<ValidationIssue> Validate(IEnumerable<ParsedRecord> parsedRecords)
        => RecordValidator.Validate(parsedRecords).ToList();

    private static ParsedRecord GetParsedrecord(int rowNumber, string reference, string parent, string name)
        => new(
            rowNumber,
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, reference),
                Name = Field.FromValue(ColumnNames.Name, name),
                Parent = Field.FromValue(ColumnNames.Parent, parent)
            },
            Array.Empty<ValidationIssue>());

    [Fact]
    public void ItAddsFieldValueRequired()
    {
        // Arrange
        var parsedRecords = new List<ParsedRecord>()
        {
            GetParsedrecord(2, "", "ovo00025", "name1"),
            GetParsedrecord(3, "REF2", "ovo00026", ""),
            GetParsedrecord(4, "", "ovo00027", "")
        };

        // Act
        var validationIssues = Validate(parsedRecords);
        // Assert
        validationIssues.Should().HaveCount(3);
        validationIssues[0].RowNumber.Should().Be(2);
        validationIssues[0].Error.Should()
            .Be("Rij ontbreekt waarde voor volgende kolommen: 'reference'.");
        validationIssues[1].RowNumber.Should().Be(3);
        validationIssues[1].Error.Should()
            .Be("Rij ontbreekt waarde voor volgende kolommen: 'name'.");
        validationIssues[2].RowNumber.Should().Be(4);
        validationIssues[2].Error.Should()
            .Be("Rij ontbreekt waarde voor volgende kolommen: 'reference', 'name'.");
    }
}
