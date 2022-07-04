namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationCapacity.WhenScheduledCommandsAreRun;

using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Organisation;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class Bugfix_Given_An_OrganisationCapacity_ValidFrom_Null_ValidTo_Null_IsActive_True
{
    [Fact]
    public async Task Then_No_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var organisationCapacity = fixture.Create<OrganisationCapacityListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, _) =>
        {
            // should be inactive -> is active (a command)
            organisationCapacity.ValidFrom = null;
            organisationCapacity.ValidTo = null;
            organisationCapacity.IsActive = true;
            testContext.OrganisationCapacityList.Add(organisationCapacity);
        });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(It.IsAny<ICommand>(), It.IsAny<IUser>()), Times.Never());
    }
}
