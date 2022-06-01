namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent;

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TestBases;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Organisation.Events;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class OrganisationParentInfoUpdatedBugfix : ListViewTestBase
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

    [Fact]
    public async Task UpdateOrganisationInfoThatIsNotTheParent_DoesNotInfluenceTheCurrentParentInTheDetail()
    {
        var organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var parentOrganisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var parentOrganisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationAParentAdded = new OrganisationParentAddedBuilder(organisationCreated.Id, parentOrganisationACreated.Id).WithValidity(null, null);
        var organisationBParentAdded = new OrganisationParentAddedBuilder(organisationCreated.Id, parentOrganisationBCreated.Id).WithValidity(new DateTime(2017, 02, 01), null);
        var organisationOrganisationParentAId = organisationAParentAdded.OrganisationOrganisationParentId;
        var organisationOrganisationParentBId = organisationBParentAdded.OrganisationOrganisationParentId;

        await HandleEvents(
            organisationCreated.Build(),
            parentOrganisationACreated.Build(),
            parentOrganisationBCreated.Build(),
            organisationAParentAdded.Build(),
            new ParentAssignedToOrganisation(organisationCreated.Id, parentOrganisationACreated.Id, organisationOrganisationParentAId),
            new OrganisationParentUpdatedBuilder(organisationOrganisationParentAId, organisationCreated.Id, parentOrganisationACreated.Id)
                .WithValidity(new DateTime(2006, 04, 01), new DateTime(2017-01-31))
                .Build(),
            new ParentClearedFromOrganisation(organisationCreated.Id, parentOrganisationACreated.Id),
            organisationBParentAdded.Build(),
            new ParentAssignedToOrganisation(organisationCreated.Id, parentOrganisationBCreated.Id, organisationOrganisationParentBId),
            new OrganisationParentUpdatedBuilder(organisationOrganisationParentBId, organisationCreated.Id, parentOrganisationBCreated.Id)
                .WithValidity(new DateTime(2017, 02, 09), null)
                .Build(),
            new OrganisationParentUpdatedBuilder(organisationOrganisationParentAId, organisationCreated.Id, parentOrganisationACreated.Id).Build()
        );

        var organisationDetailItem = Context.OrganisationDetail.Single(item => item.Id == organisationCreated.Id);

        organisationDetailItem.ParentOrganisation.Should().Be(parentOrganisationBCreated.Name);
        organisationDetailItem.ParentOrganisationId.Should().Be(parentOrganisationBCreated.Id);

        var organisationListItem = Context.OrganisationList.Single(item => item.OrganisationId == organisationCreated.Id && item.FormalFrameworkId == null);

        organisationListItem.ParentOrganisation.Should().Be(parentOrganisationBCreated.Name);
        organisationListItem.ParentOrganisationId.Should().Be(parentOrganisationBCreated.Id);
    }

    [Fact]
    public async Task UpdateOrganisationParent_InfluencesTheCurrentParentInTheDetail()
    {
        var organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var parentOrganisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var parentOrganisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var parentOrganisationCCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationAParentAdded = new OrganisationParentAddedBuilder(organisationCreated.Id, parentOrganisationACreated.Id).WithValidity(null, null);
        var organisationBParentAdded = new OrganisationParentAddedBuilder(organisationCreated.Id, parentOrganisationBCreated.Id).WithValidity(new DateTime(2017, 02, 01), null);
        var organisationOrganisationParentAId = organisationAParentAdded.OrganisationOrganisationParentId;
        var organisationOrganisationParentBId = organisationBParentAdded.OrganisationOrganisationParentId;

        await HandleEvents(
            organisationCreated.Build(),
            parentOrganisationACreated.Build(),
            parentOrganisationBCreated.Build(),
            parentOrganisationCCreated.Build(),
            organisationAParentAdded.Build(),
            new ParentAssignedToOrganisation(organisationCreated.Id, parentOrganisationACreated.Id, organisationOrganisationParentAId),
            new OrganisationParentUpdatedBuilder(organisationOrganisationParentAId, organisationCreated.Id, parentOrganisationACreated.Id)
                .WithValidity(new DateTime(2006, 04, 01), new DateTime(2017 - 01 - 31))
                .Build(),
            new ParentClearedFromOrganisation(organisationCreated.Id, parentOrganisationACreated.Id),
            organisationBParentAdded.Build(),
            new ParentAssignedToOrganisation(organisationCreated.Id, parentOrganisationBCreated.Id, organisationOrganisationParentBId),
            new OrganisationParentUpdatedBuilder(organisationOrganisationParentBId, organisationCreated.Id, parentOrganisationCCreated.Id)
                .WithPreviousParent(parentOrganisationBCreated.Id)
                .WithValidity(new DateTime(2017, 02, 09), null)
                .Build()
        );

        var organisationDetailItem = Context.OrganisationDetail.Single(item => item.Id == organisationCreated.Id);

        organisationDetailItem.ParentOrganisation.Should().Be(parentOrganisationCCreated.Name);
        organisationDetailItem.ParentOrganisationId.Should().Be(parentOrganisationCCreated.Id);

        var organisationListItem = Context.OrganisationList.Single(item => item.OrganisationId == organisationCreated.Id && item.FormalFrameworkId == null);

        organisationListItem.ParentOrganisation.Should().Be(parentOrganisationCCreated.Name);
        organisationListItem.ParentOrganisationId.Should().Be(parentOrganisationCCreated.Id);
    }
}
