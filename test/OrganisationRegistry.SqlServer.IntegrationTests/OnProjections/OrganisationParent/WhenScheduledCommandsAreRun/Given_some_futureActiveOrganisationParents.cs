namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenScheduledCommandsAreRun
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Organisation.ScheduledActions.Parent;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class Given_some_futureActiveOrganisationParents
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

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
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

            var commands = await service.GetCommands(dateTimeProviderStub.Today);

            var expectedCommands = new List<ICommand>
            {
                new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent1.OrganisationId)),
                new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent3.OrganisationId)),
                new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent4.OrganisationId)),
                new UpdateCurrentOrganisationParent(new OrganisationId(futureActiveOrganisationParent5.OrganisationId)),
            };

            commands.Should().BeEquivalentTo(expectedCommands);
        }
    }
}
