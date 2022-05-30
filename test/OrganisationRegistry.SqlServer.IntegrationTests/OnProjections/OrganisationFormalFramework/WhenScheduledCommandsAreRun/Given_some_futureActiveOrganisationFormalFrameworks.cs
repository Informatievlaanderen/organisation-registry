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
public class Given_Some_FutureActiveOrganisationFormalFrameworks
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var futureActiveOrganisationFormalFramework1 = fixture.Create<FutureActiveOrganisationFormalFrameworkListItem>();
        var futureActiveOrganisationFormalFramework2 = fixture.Create<FutureActiveOrganisationFormalFrameworkListItem>();
        var futureActiveOrganisationFormalFramework3 = fixture.Create<FutureActiveOrganisationFormalFrameworkListItem>();
        var futureActiveOrganisationFormalFramework4 = fixture.Create<FutureActiveOrganisationFormalFrameworkListItem>();
        var futureActiveOrganisationFormalFramework5 = fixture.Create<FutureActiveOrganisationFormalFrameworkListItem>();

        var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            futureActiveOrganisationFormalFramework1.ValidFrom = today.AddDays(-3);
            testContext.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFramework1);

            futureActiveOrganisationFormalFramework2.ValidFrom = today.AddDays(9);
            testContext.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFramework2);

            futureActiveOrganisationFormalFramework3.ValidFrom = today.AddDays(-5);
            testContext.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFramework3);

            futureActiveOrganisationFormalFramework4.ValidFrom = today.AddMonths(-2);
            testContext.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFramework4);

            futureActiveOrganisationFormalFramework5.ValidFrom = today;
            testContext.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFramework5);
        });

        var commands = await service.GetCommands(dateTimeProviderStub.Today);

        var expectedCommands = new List<ICommand>
        {
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(futureActiveOrganisationFormalFramework1.OrganisationId),
                new FormalFrameworkId(futureActiveOrganisationFormalFramework1.FormalFrameworkId)),
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(futureActiveOrganisationFormalFramework3.OrganisationId),
                new FormalFrameworkId(futureActiveOrganisationFormalFramework3.FormalFrameworkId)),
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(futureActiveOrganisationFormalFramework4.OrganisationId),
                new FormalFrameworkId(futureActiveOrganisationFormalFramework4.FormalFrameworkId)),
            new UpdateOrganisationFormalFrameworkParents(
                new OrganisationId(futureActiveOrganisationFormalFramework5.OrganisationId),
                new FormalFrameworkId(futureActiveOrganisationFormalFramework5.FormalFrameworkId)),
        };

        commands.Should().BeEquivalentTo(expectedCommands);
    }
}