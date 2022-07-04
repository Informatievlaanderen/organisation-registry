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
public class Bugfix_Given_Some_OrganisationCapacities_ValidFrom_NullOrPast_ValidTo_InThePast_IsActive_False
{
    [Fact]
    public async Task Then_No_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var organisationCapacity1 = fixture.Create<OrganisationCapacityListItem>();
        var organisationCapacity2 = fixture.Create<OrganisationCapacityListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            // should be inactive -> is not active (no command)
            organisationCapacity1.ValidFrom = null;
            organisationCapacity1.ValidTo = today.AddDays(-10);
            organisationCapacity1.IsActive = false;
            testContext.OrganisationCapacityList.Add(organisationCapacity1);

            organisationCapacity2.ValidFrom = today.AddDays(-30);
            organisationCapacity2.ValidTo = today.AddDays(-10);
            organisationCapacity2.IsActive = false;
            testContext.OrganisationCapacityList.Add(organisationCapacity2);
        });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(It.IsAny<ICommand>(), It.IsAny<IUser>()), Times.Never());
    }
}
