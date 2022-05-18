namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body.WhenScheduledCommandsAreRun
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Infrastructure.Commands;
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

            var (service, dateTimeProviderStub) = await ScheduledCommandsScenario.Arrange((testContext, today) =>
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

            var commands = await service.GetCommands(dateTimeProviderStub.Today);

            var expectedCommands = new List<ICommand>
            {
                new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation1.BodyId)),
                new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation3.BodyId)),
                new UpdateCurrentBodyOrganisation(new BodyId(activeBodyOrganisation4.BodyId)),
            };

            commands.Should().BeEquivalentTo(expectedCommands);
        }
    }
}
