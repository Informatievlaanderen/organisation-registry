namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationCapacity.WhenScheduledCommandsAreRun
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Organisation;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class Given_Some_OrganisationCapacities
    {
        [Fact]
        public async Task Then_The_Correct_Commands_Are_Generated()
        {
            var fixture = new Fixture();
            var organisationCapacity1 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity1B = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity1C = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity2 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity2B = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity2C = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity3 = fixture.Create<OrganisationCapacityListItem>();
            var organisationCapacity4 = fixture.Create<OrganisationCapacityListItem>();

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
            {
                // should be active -> is active (no command)
                organisationCapacity1.ValidFrom = null;
                organisationCapacity1.ValidTo = today.AddDays(20);
                organisationCapacity1.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity1);

                organisationCapacity1B.ValidFrom = null;
                organisationCapacity1B.ValidTo = today;
                organisationCapacity1B.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity1B);

                organisationCapacity1C.ValidFrom = today;
                organisationCapacity1C.ValidTo = null;
                organisationCapacity1C.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity1C);

                // should be active -> is not active (a command)
                organisationCapacity2.ValidFrom = today.AddDays(-10);
                organisationCapacity2.ValidTo = null;
                organisationCapacity2.IsActive = false;
                testContext.OrganisationCapacityList.Add(organisationCapacity2);

                organisationCapacity2B.ValidFrom = null;
                organisationCapacity2B.ValidTo = today;
                organisationCapacity2B.IsActive = false;
                testContext.OrganisationCapacityList.Add(organisationCapacity2B);

                organisationCapacity2C.ValidFrom = today;
                organisationCapacity2C.ValidTo = null;
                organisationCapacity2C.IsActive = false;
                testContext.OrganisationCapacityList.Add(organisationCapacity2C);

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
            });

            var commands = await service.GetCommands(dateTimeProviderStub.Today);

            var expectedCommands = new List<ICommand>
            {
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity2.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity2B.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity2C.OrganisationId), dateTimeProviderStub.Today),
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity3.OrganisationId), dateTimeProviderStub.Today),
            };

            commands.Should().BeEquivalentTo(expectedCommands);
        }
    }
}
