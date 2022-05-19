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
    public class Bugfix_Given_An_OrganisationCapacity_ValidFrom_Null_ValidTo_InThePast_IsActive_True
    {
        [Fact]
        public async Task Then_The_Correct_Commands_Are_Generated()
        {
            var fixture = new Fixture();
            var organisationCapacity = fixture.Create<OrganisationCapacityListItem>();

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
            {
                // should be inactive -> is active (a command)
                organisationCapacity.ValidFrom = null;
                organisationCapacity.ValidTo = today.AddDays(-20);
                organisationCapacity.IsActive = true;
                testContext.OrganisationCapacityList.Add(organisationCapacity);
            });

            var commands = await service.GetCommands(dateTimeProviderStub.Today);

            var expectedCommands = new List<ICommand>
            {
                new UpdateRelationshipValidities(new OrganisationId(organisationCapacity.OrganisationId), dateTimeProviderStub.Today),
            };

            commands.Should().BeEquivalentTo(expectedCommands);
        }
    }
}
