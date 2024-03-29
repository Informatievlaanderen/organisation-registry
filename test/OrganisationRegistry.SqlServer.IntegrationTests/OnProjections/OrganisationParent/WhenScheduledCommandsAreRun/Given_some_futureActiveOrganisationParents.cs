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
public class Given_Some_FutureActiveOrganisationParents
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();
        var futureActiveOrganisationParent1 = fixture.Create<FutureActiveOrganisationParentListItem>();
        var futureActiveOrganisationParent2 = fixture.Create<FutureActiveOrganisationParentListItem>();
        var futureActiveOrganisationParent3 = fixture.Create<FutureActiveOrganisationParentListItem>();
        var futureActiveOrganisationParent4 = fixture.Create<FutureActiveOrganisationParentListItem>();
        var futureActiveOrganisationParent5 = fixture.Create<FutureActiveOrganisationParentListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
        {
            futureActiveOrganisationParent1.ValidFrom = today.AddDays(-3);
            testContext.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParent1);

            futureActiveOrganisationParent2.ValidFrom = today.AddDays(9);
            testContext.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParent2);

            futureActiveOrganisationParent3.ValidFrom = today.AddDays(-5);
            testContext.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParent3);

            futureActiveOrganisationParent4.ValidFrom = today.AddMonths(-2);
            testContext.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParent4);

            futureActiveOrganisationParent5.ValidFrom = today;
            testContext.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParent5);
        });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent1.OrganisationId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent3.OrganisationId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent4.OrganisationId)), It.IsAny<IUser>()), Times.Once);
        commandSenderMock.Verify(sender => sender.Send(new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent5.OrganisationId)), It.IsAny<IUser>()), Times.Once);
    }
}
