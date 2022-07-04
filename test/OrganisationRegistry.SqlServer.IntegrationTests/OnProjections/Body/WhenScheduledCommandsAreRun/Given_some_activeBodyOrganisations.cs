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
public class Given_Some_ActiveBodyOrganisations
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var activeBodyOrganisation1 = fixture.Create<ActiveBodyOrganisationListItem>();
        var activeBodyOrganisation2 = fixture.Create<ActiveBodyOrganisationListItem>();
        var activeBodyOrganisation3 = fixture.Create<ActiveBodyOrganisationListItem>();
        var activeBodyOrganisation4 = fixture.Create<ActiveBodyOrganisationListItem>();
        var activeBodyOrganisation5 = fixture.Create<ActiveBodyOrganisationListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            activeBodyOrganisation1.ValidTo = today.AddDays(-3);
            testContext.ActiveBodyOrganisationList.Add(activeBodyOrganisation1);

            activeBodyOrganisation2.ValidTo = today.AddDays(9);
            testContext.ActiveBodyOrganisationList.Add(activeBodyOrganisation2);

            activeBodyOrganisation3.ValidTo = today.AddDays(-5);
            testContext.ActiveBodyOrganisationList.Add(activeBodyOrganisation3);

            activeBodyOrganisation4.ValidTo = today.AddMonths(-2);
            testContext.ActiveBodyOrganisationList.Add(activeBodyOrganisation4);

            activeBodyOrganisation5.ValidTo = today;
            testContext.ActiveBodyOrganisationList.Add(activeBodyOrganisation5);
        });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation1.BodyId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation3.BodyId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation4.BodyId)), It.IsAny<IUser>()), Times.Once);
    }
}
