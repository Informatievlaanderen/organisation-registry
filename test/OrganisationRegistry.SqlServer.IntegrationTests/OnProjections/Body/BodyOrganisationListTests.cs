namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body;

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TestBases;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class BodyOrganisationListTests : ListViewTestBase
{
    [Fact]
    public async Task BodyOrganisationAdded()
    {
        var sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

        var bodyRegistered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator);
        var organisationCreated = new OrganisationCreatedBuilder(sequentialOvoNumberGenerator);
        var bodyOrganisationAdded = new BodyOrganisationAddedBuilder(bodyRegistered.Id, organisationCreated.Id);

        await HandleEvents(
            organisationCreated.Build(),
            bodyRegistered.Build(),
            bodyOrganisationAdded.Build());

        var bodyOrganisationListItem =
            Context.BodyOrganisationList
                .SingleOrDefault(item => item.BodyOrganisationId == bodyOrganisationAdded.BodyOrganisationId);

        bodyOrganisationListItem.Should().NotBeNull();

        bodyOrganisationListItem!.OrganisationId.Should().Be(bodyOrganisationAdded.OrganisationId);
        bodyOrganisationListItem.OrganisationName.Should().Be(bodyOrganisationAdded.OrganisationName);
        bodyOrganisationListItem.BodyId.Should().Be(bodyOrganisationAdded.BodyId);
        bodyOrganisationListItem.ValidFrom.Should().Be(bodyOrganisationAdded.ValidFrom);
        bodyOrganisationListItem.ValidTo.Should().Be(bodyOrganisationAdded.ValidTo);
    }
}
