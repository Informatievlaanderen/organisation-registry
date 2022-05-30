namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFormalFramework.WhenScheduledCommandsAreRun;

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Organisation.ScheduledActions.FormalFramework;
using OrganisationRegistry.FormalFramework;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class Given_Some_ActiveOrganisationFormalFrameworks
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var activeOrganisationFormalFramework1 = fixture.Create<ActiveOrganisationFormalFrameworkListItem>();
        var activeOrganisationFormalFramework2 = fixture.Create<ActiveOrganisationFormalFrameworkListItem>();
        var activeOrganisationFormalFramework3 = fixture.Create<ActiveOrganisationFormalFrameworkListItem>();
        var activeOrganisationFormalFramework4 = fixture.Create<ActiveOrganisationFormalFrameworkListItem>();
        var activeOrganisationFormalFramework5 = fixture.Create<ActiveOrganisationFormalFrameworkListItem>();

        var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            activeOrganisationFormalFramework1.ValidTo = today.AddDays(-3);
            testContext.ActiveOrganisationFormalFrameworkList.Add(activeOrganisationFormalFramework1);

            activeOrganisationFormalFramework2.ValidTo = today.AddDays(9);
            testContext.ActiveOrganisationFormalFrameworkList.Add(activeOrganisationFormalFramework2);

            activeOrganisationFormalFramework3.ValidTo = today.AddDays(-5);
            testContext.ActiveOrganisationFormalFrameworkList.Add(activeOrganisationFormalFramework3);

            activeOrganisationFormalFramework4.ValidTo = today.AddMonths(-2);
            testContext.ActiveOrganisationFormalFrameworkList.Add(activeOrganisationFormalFramework4);

            activeOrganisationFormalFramework5.ValidTo = today;
            testContext.ActiveOrganisationFormalFrameworkList.Add(activeOrganisationFormalFramework5);
        });

        var commands = await service.GetCommands(dateTimeProviderStub.Today);

        var expectedCommands = new List<ICommand>
        {
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(activeOrganisationFormalFramework1.OrganisationId),
                new FormalFrameworkId(activeOrganisationFormalFramework1.FormalFrameworkId)),
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(activeOrganisationFormalFramework3.OrganisationId),
                new FormalFrameworkId(activeOrganisationFormalFramework3.FormalFrameworkId)),
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(activeOrganisationFormalFramework4.OrganisationId),
                new FormalFrameworkId(activeOrganisationFormalFramework4.FormalFrameworkId)),
        };

        commands.Should().BeEquivalentTo(expectedCommands);
    }
}