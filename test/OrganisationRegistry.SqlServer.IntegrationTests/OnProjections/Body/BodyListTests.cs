namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Body.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class BodyListTests : ListViewTestBase
    {
        [Fact]
        public async Task BodyRegistered()
        {
            var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

            var body1Registered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator).Build();
            var body2Registered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator).Build();

            await HandleEvents(
                body1Registered,
                body2Registered);

            AssertBodyListItem(body1Registered);
            AssertBodyListItem(body2Registered);
        }

        [Fact]
        public async Task BodyAssignedToOrganisation()
        {
            var sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
            var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

            var bodyRegistered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator);
            var organisationCreated = new OrganisationCreatedBuilder(sequentialOvoNumberGenerator);
            var bodyOrganisationAdded = new BodyOrganisationAddedBuilder(bodyRegistered.Id, organisationCreated.Id);

            var bodyAssignedToOrganisation = new BodyAssignedToOrganisation(
                bodyRegistered.Id, bodyRegistered.Name,
                organisationCreated.Id, organisationCreated.Name,
                bodyOrganisationAdded.BodyOrganisationId);

            await HandleEvents(
                organisationCreated.Build(),
                bodyRegistered.Build(),
                bodyOrganisationAdded.Build(),
                bodyAssignedToOrganisation);

            AssertBodyListItem(bodyRegistered.Build(), bodyAssignedToOrganisation);
        }

        private void AssertBodyListItem(BodyRegistered bodyRegistered)
        {
            var bodyListItem = Context.BodyList.SingleOrDefault(item => item.Id == bodyRegistered.BodyId);

            bodyListItem.Should().NotBeNull();

            bodyListItem!.Id.Should().Be(bodyRegistered.BodyId);
            bodyListItem.Name.Should().Be(bodyRegistered.Name);
            bodyListItem.ShortName.Should().Be(bodyRegistered.ShortName);

            bodyListItem.Organisation.Should().BeNullOrEmpty();
            bodyListItem.OrganisationId.Should().BeNull();
        }

        private void AssertBodyListItem(BodyRegistered bodyRegistered, BodyAssignedToOrganisation bodyAssignedToOrganisation)
        {
            var bodyListItem = Context.BodyList.SingleOrDefault(item => item.Id == bodyRegistered.BodyId);

            bodyListItem.Should().NotBeNull();

            bodyListItem!.Id.Should().Be(bodyRegistered.BodyId);
            bodyListItem.Name.Should().Be(bodyRegistered.Name);
            bodyListItem.ShortName.Should().Be(bodyRegistered.ShortName);

            bodyListItem.Organisation.Should().Be(bodyAssignedToOrganisation.OrganisationName);
            bodyListItem.OrganisationId.Should().Be(bodyAssignedToOrganisation.OrganisationId);
        }
    }
}
