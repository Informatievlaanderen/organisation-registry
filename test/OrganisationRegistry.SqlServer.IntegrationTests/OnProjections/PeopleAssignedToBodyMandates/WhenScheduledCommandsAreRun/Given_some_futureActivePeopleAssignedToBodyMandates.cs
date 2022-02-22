namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.PeopleAssignedToBodyMandates.WhenScheduledCommandsAreRun
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.Infrastructure.Commands;
    using SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class Given_Some_FutureActivePeopleAssignedToBodyMandates
    {
        [Fact]
        public async Task Then_The_Correct_Commands_Are_Generated()
        {
            var fixture = new Fixture();

            // body with 3 MandateAssignments
            var body1Id = fixture.Create<Guid>();
            var activePeopleAssignedToBodyMandate1A = new FuturePeopleAssignedToBodyMandatesListItem
            {
                BodyId = body1Id,
                BodySeatId = fixture.Create<Guid>(),
                BodyMandateId = fixture.Create<Guid>()
            };
            var activePeopleAssignedToBodyMandate1B = new FuturePeopleAssignedToBodyMandatesListItem
            {
                BodyId = body1Id,
                BodySeatId = fixture.Create<Guid>(),
                BodyMandateId = fixture.Create<Guid>()
            };
            var activePeopleAssignedToBodyMandate1C = new FuturePeopleAssignedToBodyMandatesListItem
            {
                BodyId = body1Id,
                BodySeatId = fixture.Create<Guid>(),
                BodyMandateId = fixture.Create<Guid>()
            };

            var activePeopleAssignedToBodyMandate2 = fixture.Create<FuturePeopleAssignedToBodyMandatesListItem>();
            var activePeopleAssignedToBodyMandate3 = fixture.Create<FuturePeopleAssignedToBodyMandatesListItem>();
            var activePeopleAssignedToBodyMandate4 = fixture.Create<FuturePeopleAssignedToBodyMandatesListItem>();
            var activePeopleAssignedToBodyMandate5 = fixture.Create<FuturePeopleAssignedToBodyMandatesListItem>();

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
            {
                activePeopleAssignedToBodyMandate1A.ValidFrom = today.AddDays(-3);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1A);
                activePeopleAssignedToBodyMandate1B.ValidFrom = today.AddDays(5);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1B);
                activePeopleAssignedToBodyMandate1C.ValidFrom = today.AddDays(-9);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate1C);

                activePeopleAssignedToBodyMandate2.ValidFrom = today.AddDays(9);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate2);

                activePeopleAssignedToBodyMandate3.ValidFrom = today.AddDays(-5);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate3);

                activePeopleAssignedToBodyMandate4.ValidFrom = today.AddMonths(-2);
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate4);

                activePeopleAssignedToBodyMandate5.ValidFrom = today;
                testContext.FuturePeopleAssignedToBodyMandatesList.Add(activePeopleAssignedToBodyMandate5);
            });

            var commands = (await service.GetCommands(dateTimeProviderStub.Today)).ToList();

            var expectedCommands = new List<ICommand>
            {
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(body1Id),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate1A.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate1A.BodyMandateId)),
                        new(new BodySeatId(activePeopleAssignedToBodyMandate1C.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate1C.BodyMandateId)),
                    }
                ),
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(activePeopleAssignedToBodyMandate3.BodyId),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate3.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate3.BodyMandateId)),
                    }
                ),
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(activePeopleAssignedToBodyMandate4.BodyId),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate4.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate4.BodyMandateId)),
                    }
                ),
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(activePeopleAssignedToBodyMandate5.BodyId),
                    new List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)>
                    {
                        new(new BodySeatId(activePeopleAssignedToBodyMandate4.BodySeatId), new BodyMandateId(activePeopleAssignedToBodyMandate5.BodyMandateId)),
                    }
                ),
            };

            commands.Should().BeEquivalentTo(expectedCommands);

            var nestedCommand = commands.First().As<UpdateCurrentPersonAssignedToBodyMandate>();
            nestedCommand.MandatesToUpdate.Should().BeEquivalentTo(expectedCommands.First().As<UpdateCurrentPersonAssignedToBodyMandate>().MandatesToUpdate);
        }
    }
}
