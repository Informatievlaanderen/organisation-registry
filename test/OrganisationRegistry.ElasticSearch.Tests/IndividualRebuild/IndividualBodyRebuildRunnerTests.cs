namespace OrganisationRegistry.ElasticSearch.Tests.IndividualRebuildRunners;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Body.Events;
using Bodies;
using OrganisationRegistry.ElasticSearch.Client;
using OrganisationRegistry.ElasticSearch.Projections;
using OrganisationRegistry.ElasticSearch.Projections.Infrastructure;
using Scenario;
using Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using OrganisationRegistry.SqlServer.Infrastructure;
using Projections.IndividualRebuild;
using SqlServer.ProjectionState;
using Xunit;

[Collection(nameof(ElasticSearchFixture))]
public class IndividualBodyRebuildRunnerTests
    : IndividualRebuildRunnerTestBase<OrganisationRegistry.Body.Body, BodyDocument, BodyToRebuild>
{
    public IndividualBodyRebuildRunnerTests(ElasticSearchFixture fixture) : base(fixture) { }

    protected override IndividualRebuildRunnerConfig<OrganisationRegistry.Body.Body, BodyDocument, BodyToRebuild> Config
        => IndividualRebuildRunnerConfigs.Body;

    protected override IndividualRebuildRunner<OrganisationRegistry.Body.Body, BodyDocument, BodyToRebuild> CreateRunner(
        IEventStore eventStore, IContextFactory contextFactory, IProjectionStates projectionStates,
        ElasticBus bus, Elastic elastic, ElasticBusRegistrar busRegistrar)
        => new(NullLogger.Instance, eventStore, contextFactory, projectionStates, bus, elastic, busRegistrar, Config);

    protected override List<IEnvelope> CreateEnvelopes(Guid aggregateId)
    {
        var scenario = new BodyScenario(aggregateId);
        return new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(1, string.Empty, string.Empty, string.Empty, string.Empty),
            scenario.Create<BodyRegistered>().ToEnvelope(2, string.Empty, string.Empty, string.Empty, string.Empty),
        };
    }

    protected override void SeedToRebuild(OrganisationRegistryContext context, Guid aggregateId)
        => context.BodiesToRebuild.Add(new BodyToRebuild { BodyId = aggregateId });

    protected override async Task Verify(
        ElasticSearchFixture fixture, IContextFactory contextFactory, Guid aggregateId)
    {
        var document = fixture.Elastic.ReadClient.Get<BodyDocument>(aggregateId);
        document.Source.Should().NotBeNull();

        await using var ctx = contextFactory.Create();
        var remaining = await ctx.BodiesToRebuild.ToListAsync();
        remaining.Should().BeEmpty();
    }
}
