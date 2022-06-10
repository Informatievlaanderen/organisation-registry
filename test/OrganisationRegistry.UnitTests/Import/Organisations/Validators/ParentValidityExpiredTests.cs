namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using System;
using System.Collections.Immutable;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using SqlServer.Organisation;
using Xunit;

public class ParentValidityExpiredTests
{
    private readonly DateTime _today;
    private readonly IFixture _fixture = new Fixture();
    private readonly DateTime _yesterday;
    private readonly DateTime _tomorrow;

    public ParentValidityExpiredTests()
    {
        _today = _fixture.Create<DateTime>();
        _yesterday = _today.AddDays(-1);
        _tomorrow = _today.AddDays(1);
    }

    private static DeserializedRecord GetDeserializedRecordWithParent(string parent)
        => new()
        {
            Parent = Field.FromValue(ColumnNames.Parent, parent),
        };

    private ImmutableList<OrganisationListItem> OrganisationsCache
        => ImmutableList.Create<OrganisationListItem>()
            .Add(
                new OrganisationListItem
                {
                    OvoNumber = "Ovo000001",
                    ValidTo = _tomorrow
                })
            .Add(
                new OrganisationListItem
                {
                    OvoNumber = "Ovo000002",
                    ValidTo = _yesterday
                });

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoValue()
    {
        var record = new DeserializedRecord { Parent = Field.NoValue(ColumnNames.Parent) };

        var issue = ParentValidityExpired.Validate(
            OrganisationsCache,
            DateOnly.FromDateTime(_today),
            1,
            record);

        issue.Should().BeNull();
    }


    [Fact]
    public void ReturnsEmpty_WhenParentDoesNotExistInList()
    {
        var record = GetDeserializedRecordWithParent("doesNotExist");

        var issue = ParentValidityExpired.Validate(
            OrganisationsCache,
            DateOnly.FromDateTime(_today),
            1,
            record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenParentIsStillValid()
    {
        var record = GetDeserializedRecordWithParent("Ovo000001");

        var issue = ParentValidityExpired.Validate(
            OrganisationsCache,
            DateOnly.FromDateTime(_today),
            1,
            record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsIssue_WhenParentIsNoLongerValid()
    {
        var record = GetDeserializedRecordWithParent("Ovo000002");

        var issue = ParentValidityExpired.Validate(
            OrganisationsCache,
            DateOnly.FromDateTime(_today),
            1,
            record);
        issue.Should().BeEquivalentTo(
            new ValidationIssue(1, ParentValidityExpired.FormatMessage("'Ovo000002'")));
    }
}
