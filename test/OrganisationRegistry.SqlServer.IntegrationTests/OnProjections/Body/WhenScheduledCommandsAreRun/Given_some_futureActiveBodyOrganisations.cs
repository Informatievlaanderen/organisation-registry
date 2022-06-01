namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body.WhenScheduledCommandsAreRun;

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Commands;
using SqlServer.Body.ScheduledActions.Organisation;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class Given_Some_FutureActiveBodyOrganisations
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var futureActiveBodyOrganisation1 = fixture.Create<FutureActiveBodyOrganisationListItem>();
        var futureActiveBodyOrganisation2 = fixture.Create<FutureActiveBodyOrganisationListItem>();
        var futureActiveBodyOrganisation3 = fixture.Create<FutureActiveBodyOrganisationListItem>();
        var futureActiveBodyOrganisation4 = fixture.Create<FutureActiveBodyOrganisationListItem>();
        var futureActiveBodyOrganisation5 = fixture.Create<FutureActiveBodyOrganisationListItem>();

        var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            futureActiveBodyOrganisation1.ValidFrom = today.AddDays(-3);
            testContext.FutureActiveBodyOrganisationList.Add(futureActiveBodyOrganisation1);

            futureActiveBodyOrganisation2.ValidFrom = today.AddDays(9);
            testContext.FutureActiveBodyOrganisationList.Add(futureActiveBodyOrganisation2);

            futureActiveBodyOrganisation3.ValidFrom = today.AddDays(-5);
            testContext.FutureActiveBodyOrganisationList.Add(futureActiveBodyOrganisation3);

            futureActiveBodyOrganisation4.ValidFrom = today.AddMonths(-2);
            testContext.FutureActiveBodyOrganisationList.Add(futureActiveBodyOrganisation4);

            futureActiveBodyOrganisation5.ValidFrom = today;
            testContext.FutureActiveBodyOrganisationList.Add(futureActiveBodyOrganisation5);
        });

        var commands = await service.GetCommands(dateTimeProviderStub.Today);

        var expectedCommands = new List<ICommand>
        {
            new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation1.BodyId)),
            new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation3.BodyId)),
            new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation4.BodyId)),
            new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation5.BodyId)),
        };

        commands.Should().BeEquivalentTo(expectedCommands);
    }
}
