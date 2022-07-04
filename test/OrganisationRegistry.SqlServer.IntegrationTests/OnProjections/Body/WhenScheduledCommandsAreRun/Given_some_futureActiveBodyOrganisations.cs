namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body.WhenScheduledCommandsAreRun;

using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Authorization;
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

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
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

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation1.BodyId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation3.BodyId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation4.BodyId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(futureActiveBodyOrganisation5.BodyId)), It.IsAny<IUser>()), Times.Once);
    }
}
