namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using System.Collections.Immutable;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using SqlServer.Organisation;
using Xunit;

public class ParentWithOvonumberAlreadyhasDaughterWithSameNameTests
{
    private static DeserializedRecord GetDeserializedRecordWithName(string name)
        => new()
        {
            Parent = Field.NoValue(ColumnNames.Parent),
            Name = Field.FromValue(ColumnNames.Name, name)
        };

    private static DeserializedRecord GetDeserializedRecordWithParentAndName(string parent, string name)
        => new()
        {
            Parent = Field.FromValue(ColumnNames.Parent, parent),
            Name = Field.FromValue(ColumnNames.Name, name)
        };

    private static ImmutableList<OrganisationListItem> OrganisationsCache
        => ImmutableList.Create<OrganisationListItem>()
            .Add(
                new OrganisationListItem
                {
                    ParentOrganisationOvoNumber = "Ovo000001",
                    Name = "child1"
                })
            .Add(
                new OrganisationListItem
                {
                    ParentOrganisationOvoNumber = "Ovo000001",
                    Name = "child2"
                })
            .Add(
                new OrganisationListItem
                {
                    ParentOrganisationOvoNumber = "Ovo000002",
                    Name = "child1"
                });

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoValue()
    {
        var record = GetDeserializedRecordWithName("name");

        var issue = ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoChildren()
    {
        var record = GetDeserializedRecordWithParentAndName("OvonotInList", "name");

        var issue = ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoOvonumber()
    {
        var record = GetDeserializedRecordWithParentAndName("NotAnOvonumber", "name");

        var issue = ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenParentHasNoChildrenWithSameName()
    {
        var record = GetDeserializedRecordWithParentAndName("Ovo000001", "notAChildName");

        var issue = ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(OrganisationsCache, 1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsIssue_WhenParentHasAChildWithSameName()
    {
        var record = GetDeserializedRecordWithParentAndName("Ovo000001", "child1");

        var issue = ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(OrganisationsCache, 1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(1, ParentWithOvonumberAlreadyHasDaughterWithSameName.FormatMessage("Ovo000001", "child1")));
    }


}
