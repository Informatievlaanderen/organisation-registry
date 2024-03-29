﻿namespace OrganisationRegistry.UnitTests.Import.Organisations.Create;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Validation;
using OrganisationRegistry.SqlServer.Organisation;
using Tests.Shared.TestDataBuilders;
using Xunit;

public class WhenValidatingTheCsvImportFileContent
{
    private static List<ValidationIssue> Validate(IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
    {
        var fixture = new Fixture();
        var today = DateOnly.FromDateTime(fixture.Create<DateTime>());

        var importCache = FakeImportCache.Create(
            new List<OrganisationListItem>
            {
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000025" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000026" },
                new() { Name = fixture.Create<string>(), OvoNumber = "OVO000027" },
            });
        return ImportRecordValidator.Validate(importCache, today, parsedRecords).Items.ToList();
    }

    private static ParsedRecord<DeserializedRecord> GetParsedrecord(int rowNumber, string reference, string parent, string name)
        => new(
            rowNumber,
            new CreateOrganisationsDeserializedRecordBuilder(reference, name).WithParent(parent),
            Array.Empty<ValidationIssue>());

    [Fact]
    public void ItAddsFieldValueRequired()
    {
        // Arrange
        var parsedRecords = new List<ParsedRecord<DeserializedRecord>>()
        {
            GetParsedrecord(2, "", "ovo000025", "name1"),
            GetParsedrecord(3, "REF2", "ovo000026", ""),
            GetParsedrecord(4, "", "ovo000027", ""),
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
