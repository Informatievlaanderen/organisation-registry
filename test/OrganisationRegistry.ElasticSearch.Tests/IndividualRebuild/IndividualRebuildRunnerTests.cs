namespace OrganisationRegistry.ElasticSearch.Tests.IndividualRebuildRunners;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Organisations;
using OrganisationRegistry.ElasticSearch.Client;
using OrganisationRegistry.ElasticSearch.Projections;
using OrganisationRegistry.ElasticSearch.Projections.Infrastructure;
using OrganisationRegistry.ElasticSearch.Projections.Organisations;
using Scenario;
using Infrastructure.Events;
using Organisation.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using OrganisationRegistry.SqlServer.Infrastructure;
using Projections.IndividualRebuild;
using SqlServer.ProjectionState;
using Xunit;

[Collection(nameof(ElasticSearchFixture))]
public class IndividualRebuildRunnerTests
    : IndividualRebuildRunnerTestBase<OrganisationRegistry.Organisation.Organisation, OrganisationDocument, OrganisationToRebuild>
{
    public IndividualRebuildRunnerTests(ElasticSearchFixture fixture) : base(fixture) { }

    protected override IndividualRebuildRunnerConfig<OrganisationRegistry.Organisation.Organisation, OrganisationDocument, OrganisationToRebuild> Config
        => IndividualRebuildRunnerConfigs.Organisation;

    protected override IndividualRebuildRunner<OrganisationRegistry.Organisation.Organisation, OrganisationDocument, OrganisationToRebuild> CreateRunner(
        IEventStore eventStore, IContextFactory contextFactory, IProjectionStates projectionStates,
        ElasticBus bus, Elastic elastic, ElasticBusRegistrar busRegistrar)
        => new(NullLogger.Instance, eventStore, contextFactory, projectionStates, bus, elastic, busRegistrar, Config);

    protected override List<IEnvelope> CreateEnvelopes(Guid aggregateId)
    {
        var scenario = new OrganisationScenario(aggregateId);
        return new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(1, string.Empty, string.Empty, string.Empty, string.Empty),
            scenario.Create<OrganisationCreated>().ToEnvelope(2, string.Empty, string.Empty, string.Empty, string.Empty),
        };
    }

    protected override void SeedToRebuild(OrganisationRegistryContext context, Guid aggregateId)
        => context.OrganisationsToRebuild.Add(new OrganisationToRebuild { OrganisationId = aggregateId });

    protected override async Task Verify(
        ElasticSearchFixture fixture, IContextFactory contextFactory, Guid aggregateId)
    {
        var document = fixture.Elastic.ReadClient.Get<OrganisationDocument>(aggregateId);
        document.Source.Should().NotBeNull();

        await using var ctx = contextFactory.Create();
        var remaining = await ctx.OrganisationsToRebuild.ToListAsync();
        remaining.Should().BeEmpty();
    }
}
