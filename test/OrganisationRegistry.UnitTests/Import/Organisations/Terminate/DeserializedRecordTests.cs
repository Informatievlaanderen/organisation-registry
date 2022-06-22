namespace OrganisationRegistry.UnitTests.Import.Organisations.Terminate;

using System;
using AutoFixture;
using FluentAssertions;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.StopOrganisations;
using Xunit;

public class DeserializedRecordTests
{
    [Fact]
    public void WhenGivenADeserializedRecord_ItProducesACorrectCommandItem()
    {
        var fixture = new Fixture();

        var expectedOvoNumber = fixture.Create<string>();
        var expectedName = fixture.Create<string>();
        var expectedOrganisationEnd = fixture.Create<DateTime>();

        var expectedOrganisationId = fixture.Create<Guid>();

        var deserializedRecord = new DeserializedRecord
        {
            OvoNumber = Field.FromValue(ColumnNames.OvoNumber, expectedOvoNumber),
            Name = Field.FromValue(ColumnNames.Name, expectedName),
            Organisation_End = Field.FromValue(ColumnNames.Organisation_End, expectedOrganisationEnd.ToString("yyyy-MM-dd")),
        };

        var commandItem = deserializedRecord.ToCommandItem(expectedOrganisationId);

        commandItem.OrganisationId.Should().Be(expectedOrganisationId);
        commandItem.Organisation_End.Should().Be(DateOnly.FromDateTime(expectedOrganisationEnd));
    }
}
