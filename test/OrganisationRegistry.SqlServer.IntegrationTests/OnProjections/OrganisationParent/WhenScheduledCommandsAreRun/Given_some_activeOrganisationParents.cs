﻿namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenScheduledCommandsAreRun;

using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Organisation.ScheduledActions.Parent;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class Given_Some_ActiveOrganisationParents
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var activeOrganisationParent1 = fixture.Create<ActiveOrganisationParentListItem>();
        var activeOrganisationParent2 = fixture.Create<ActiveOrganisationParentListItem>();
        var activeOrganisationParent3 = fixture.Create<ActiveOrganisationParentListItem>();
        var activeOrganisationParent4 = fixture.Create<ActiveOrganisationParentListItem>();
        var activeOrganisationParent5 = fixture.Create<ActiveOrganisationParentListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            activeOrganisationParent1.ValidTo = today.AddDays(-3);
            testContext.ActiveOrganisationParentList.Add(activeOrganisationParent1);

            activeOrganisationParent2.ValidTo = today.AddDays(9);
            testContext.ActiveOrganisationParentList.Add(activeOrganisationParent2);

            activeOrganisationParent3.ValidTo = today.AddDays(-5);
            testContext.ActiveOrganisationParentList.Add(activeOrganisationParent3);

            activeOrganisationParent4.ValidTo = today.AddMonths(-2);
            testContext.ActiveOrganisationParentList.Add(activeOrganisationParent4);

            activeOrganisationParent5.ValidTo = today;
            testContext.ActiveOrganisationParentList.Add(activeOrganisationParent5);
        });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(activeOrganisationParent1.OrganisationId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(activeOrganisationParent3.OrganisationId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(activeOrganisationParent4.OrganisationId)), It.IsAny<IUser>()), Times.Once);
    }
}
