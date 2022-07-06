namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using System.Collections.Immutable;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations.Validators;
using Api.HostedServices.ProcessImportedFiles.Validation;
using FluentAssertions;
using SqlServer.Organisation;
using Xunit;

public class ParentWithOvonumberNotFoundTests
{
    private static DeserializedRecord GetDeserializedRecordWithParent(string parent)
        => new()
        {
            Parent = Field.FromValue(ColumnNames.Parent, parent),
        };

    private static ImmutableList<OrganisationListItem> OrganisationsCache
        => ImmutableList.Create<OrganisationListItem>()
            .Add(
                new OrganisationListItem
                {
                    OvoNumber = "Ovo000001",
                })
            .Add(
                new OrganisationListItem
                {
                    OvoNumber = "Ovo000002",
                });

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoValue()
    {
        var record = new DeserializedRecord { Parent = Field.NoValue(ColumnNames.Parent) };

        var issue = ParentWithOvonumberNotFound.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }


    [Fact]
    public void ReturnsEmpty_WhenParentExistsInList()
    {
        var record = GetDeserializedRecordWithParent("Ovo000001");

        var issue = ParentWithOvonumberNotFound.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsIssue_WhenParentDoesNotExistInList()
    {
        var record = GetDeserializedRecordWithParent("OvonotInList");

        var issue = ParentWithOvonumberNotFound.Validate(OrganisationsCache, 1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(1, ParentWithOvonumberNotFound.FormatMessage("'OvonotInList'")));
    }

    [Fact]
    public void Ignores_WhenParentIsNotAnOvonumber()
    {
        var record = GetDeserializedRecordWithParent("notAnOvonumber");

        var issue = ParentWithOvonumberNotFound.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }
}
