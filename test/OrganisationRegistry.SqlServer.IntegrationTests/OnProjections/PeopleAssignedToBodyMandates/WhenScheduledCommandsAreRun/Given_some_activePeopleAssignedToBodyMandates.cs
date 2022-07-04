namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.PeopleAssignedToBodyMandates.WhenScheduledCommandsAreRun;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class Given_Some_ActivePeopleAssignedToBodyMandates
{
    [Fact]
    public async Task Then_The_Correct_Commands_Are_Generated()
    {
        var fixture = new Fixture();

        // body with 3 MandateAssignments
        //var activePeopleAssignedToBodyMandate1 = fixture.Create<ActivePeopleAssignedToBodyMandateListItem>();
        var body1Id = fixture.Create<Guid>();
        var activePeopleAssignedToBodyMandate1A = new ActivePeopleAssignedToBodyMandateListItem
        {
            BodyId = body1Id,
            BodySeatId = fixture.Create<Guid>(),
            BodyMandateId = fixture.Create<Guid>(),
            PersonFullName = "test",
        };
        var activePeopleAssignedToBodyMandate1B = new ActivePeopleAssignedToBodyMandateListItem
        {
            BodyId = body1Id,
            BodySeatId = fixture.Create<Guid>(),
            BodyMandateId = fixture.Create<Guid>(),
            PersonFullName = "test",
        };
        var activePeopleAssignedToBodyMandate1C = new ActivePeopleAssignedToBodyMandateListItem
        {
            BodyId = body1Id,
            BodySeatId = fixture.Create<Guid>(),
            BodyMandateId = fixture.Create<Guid>(),
            PersonFullName = "test",
        };

        var activePeopleAssignedToBodyMandate2 = fixture.Create<ActivePeopleAssignedToBodyMandateListItem>();
        var activePeopleAssignedToBodyMandate3 = fixture.Create<ActivePeopleAssignedToBodyMandateListItem>();
        var activePeopleAssignedToBodyMandate4 = fixture.Create<ActivePeopleAssignedToBodyMandateListItem>();
        var activePeopleAssignedToBodyMandate5 = fixture.Create<ActivePeopleAssignedToBodyMandateListItem>();

        var (service, dateTimeProviderStub, commandSenderMock) = await ScheduledCommandsScenario.Arrange(
            (testContext, today) =>
            {
                activePeopleAssignedToBodyMandate1A.ValidTo = today.AddDays(-3);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1A);
                activePeopleAssignedToBodyMandate1B.ValidTo = today.AddDays(5);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1B);
                activePeopleAssignedToBodyMandate1C.ValidTo = today.AddDays(-9);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1C);

                activePeopleAssignedToBodyMandate2.ValidTo = today.AddDays(9);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate2);

                activePeopleAssignedToBodyMandate3.ValidTo = today.AddDays(-5);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate3);

                activePeopleAssignedToBodyMandate4.ValidTo = today.AddMonths(-2);
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate4);

                activePeopleAssignedToBodyMandate5.ValidTo = today;
                testContext.ActivePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate5);
            });

        await service.SendCommands(dateTimeProviderStub.Today, CancellationToken.None);

        commandSenderMock.Verify(
            sender => sender.Send(
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(body1Id),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate1A.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate1A.BodyMandateId)),
                        new(new BodySeatId(activePeopleAssignedToBodyMandate1C.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate1C.BodyMandateId)),
                    }
                ),
                It.IsAny<IUser>()),
            Times.Once);
        commandSenderMock.Verify(
            sender => sender.Send(
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(activePeopleAssignedToBodyMandate3.BodyId),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate3.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate3.BodyMandateId)),
                    }
                ),
                It.IsAny<IUser>()),
            Times.Once);
        commandSenderMock.Verify(
            sender => sender.Send(
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(activePeopleAssignedToBodyMandate4.BodyId),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate4.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate4.BodyMandateId)),
                    }
                ),
                It.IsAny<IUser>()),
            Times.Once);
    }
}
