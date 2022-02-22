namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenScheduledCommandsAreRun
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Organisation;
    using Organisation.ScheduledActions.Parent;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class Given_some_organisationCapacities
    {
        [Fact]
        public async Task Then_The_Correct_Commands_Are_Generated()
        {
            var fixture = new Fixture();
            var organisationCapacity1 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity2 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity3 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity4 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity5 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity6 = fixture.Create<OrganisationCapacityListItem>();

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
            {
                // should be active -> is active (no command)
                organisationCapacity1.ValidFrom = null;
                organisationCapacity1.ValidTo = today.AddDays(20);
                organisationCapacity1.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity1);

                // should be active -> is not active (a command)
                organisationCapacity2.ValidFrom = today.AddDays(-10);
                organisationCapacity2.ValidTo = null;
                organisationCapacity2.IsActive = false;
                testContext.OrganisationCapacityList.Add(organisationCapacity2);

                // should be inactive -> is active (a command)
                organisationCapacity3.ValidFrom = null;
                organisationCapacity3.ValidTo = today.AddDays(-20);
                organisationCapacity3.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity3);

                // should be inactive -> is not active (no command)
                organisationCapacity4.ValidFrom = today.AddDays(10);
                organisationCapacity4.ValidTo = null;
                organisationCapacity4.IsActive = false;
                testContext.OrganisationCapacityList.Add(organisationCapacity4);

                // should be active -> undetermined (a command)
                organisationCapacity5.ValidFrom = null;
                organisationCapacity5.ValidTo = today.AddDays(10);
                organisationCapacity5.IsActive = null;
                testContext.OrganisationCapacityList.Add(organisationCapacity5);

                // should be inactive -> undetermined (a command)
                organisationCapacity6.ValidFrom = today.AddDays(10);
                organisationCapacity6.ValidTo = null;
                organisationCapacity6.IsActive = null;
                testContext.OrganisationCapacityList.Add(organisationCapacity6);
            });

            var commands = await service.GetCommands(dateTimeProviderStub.Today);

            var expectedCommands = new List<ICommand>
            {
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity2.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity3.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity5.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity6.OrganisationId), dateTimeProviderStub.Today),
            };

            commands.Should().BeEquivalentTo(expectedCommands);
        }
    }
}
