namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using OrganisationRegistry.Body.Events;
    using SqlServer.Body;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class BodyDetailTests: ListViewTestBase
    {
        [Fact]
        public async Task WhenNoBodyRegistered()
        {
            var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

            var bodyRegistered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator).Build();

            await HandleEvents(bodyRegistered);

            Context.BodyDetail.Single(item => item.Id == bodyRegistered.BodyId)
                .Should().BeEquivalentTo(new BodyDetail
                {
                    Id = bodyRegistered.BodyId,
                    Name = bodyRegistered.Name,
                    ShortName = bodyRegistered.ShortName,
                    Description = bodyRegistered.Description,
                    BodyNumber = bodyRegistered.BodyNumber,
                    Organisation = null,
                    OrganisationId = null,
                    FormalValidFrom = bodyRegistered.FormalValidFrom,
                    FormalValidTo = bodyRegistered.FormalValidTo,
                    IsLifecycleValid = false,
                    BalancedParticipationExceptionMeasure = null,
                    BalancedParticipationExtraRemark = null,
                    IsBalancedParticipationObligatory = null,
                });
        }

        [Fact]
        public async Task WhenBodyParticipationChangedToTrue()
        {
            var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

            var bodyRegistered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator).Build();
            var bodyBalancedParticipationChanged = new BodyBalancedParticipationChanged(
                bodyId: bodyRegistered.BodyId,
                balancedParticipationObligatory:true,
                balancedParticipationExtraRemark: "Remark",
                balancedParticipationExceptionMeasure: "Measure",
                previousBalancedParticipationObligatory: false,
                previousBalancedParticipationExtraRemark: string.Empty,
                previousBalancedParticipationExceptionMeasure: string.Empty);

            await HandleEvents(
                bodyRegistered,
                bodyBalancedParticipationChanged);

            Context.BodyDetail.Single(item => item.Id == bodyRegistered.BodyId)
                .Should().BeEquivalentTo(new BodyDetail
                {
                    Id = bodyRegistered.BodyId,
                    Name = bodyRegistered.Name,
                    ShortName = bodyRegistered.ShortName,
                    Description = bodyRegistered.Description,
                    BodyNumber = bodyRegistered.BodyNumber,
                    Organisation = null,
                    OrganisationId = null,
                    FormalValidFrom = bodyRegistered.FormalValidFrom,
                    FormalValidTo = bodyRegistered.FormalValidTo,
                    IsLifecycleValid = false,
                    BalancedParticipationExceptionMeasure = "Measure",
                    BalancedParticipationExtraRemark = "Remark",
                    IsBalancedParticipationObligatory = true,
                });
        }

        [Fact]
        public async Task WhenBodyParticipationChangedToFalse()
        {
            var sequentialBodyNumberGenerator = new SequentialBodyNumberGenerator();

            var bodyRegistered = new BodyRegisteredBuilder(sequentialBodyNumberGenerator).Build();
            var bodyBalancedParticipationChanged = new BodyBalancedParticipationChanged(
                bodyId: bodyRegistered.BodyId,
                balancedParticipationObligatory:false,
                balancedParticipationExtraRemark: "Remark",
                balancedParticipationExceptionMeasure: "Measure",
                previousBalancedParticipationObligatory: false,
                previousBalancedParticipationExtraRemark: string.Empty,
                previousBalancedParticipationExceptionMeasure: string.Empty);

            await HandleEvents(
                bodyRegistered,
                bodyBalancedParticipationChanged);

            Context.BodyDetail.Single(item => item.Id == bodyRegistered.BodyId)
                .Should().BeEquivalentTo(new BodyDetail
                {
                    Id = bodyRegistered.BodyId,
                    Name = bodyRegistered.Name,
                    ShortName = bodyRegistered.ShortName,
                    Description = bodyRegistered.Description,
                    BodyNumber = bodyRegistered.BodyNumber,
                    Organisation = null,
                    OrganisationId = null,
                    FormalValidFrom = bodyRegistered.FormalValidFrom,
                    FormalValidTo = bodyRegistered.FormalValidTo,
                    IsLifecycleValid = false,
                    BalancedParticipationExceptionMeasure = "Measure",
                    BalancedParticipationExtraRemark = "Remark",
                    IsBalancedParticipationObligatory = false,
                });
        }
    }
}
