namespace OrganisationRegistry.ElasticSearch.Tests.IndividualRebuildRunners;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using People;
using OrganisationRegistry.ElasticSearch.Client;
using OrganisationRegistry.ElasticSearch.Projections;
using OrganisationRegistry.ElasticSearch.Projections.Infrastructure;
using OrganisationRegistry.ElasticSearch.Projections.People;
using Scenario;
using Infrastructure.Events;
using Person.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using Xunit;

[Collection(nameof(ElasticSearchFixture))]
public class IndividualPersonRebuildRunnerTests
    : IndividualRebuildRunnerTestBase<OrganisationRegistry.Person.Person>
{
    public IndividualPersonRebuildRunnerTests(ElasticSearchFixture fixture) : base(fixture) { }

    protected override Type[] EventHandlers => PeopleRunner.EventHandlers;

    protected override Func<Task> CreateRunner(
        IEventStore eventStore, IContextFactory contextFactory, IProjectionStates projectionStates,
        ElasticBus bus, Elastic elastic, ElasticBusRegistrar busRegistrar)
        => new IndividualPersonRebuildRunner(
            new NullLogger<IndividualPersonRebuildRunner>(), eventStore, contextFactory, projectionStates, bus, elastic, busRegistrar).Run;

    protected override List<IEnvelope> CreateEnvelopes(Guid aggregateId)
    {
        var scenario = new PersonScenario(aggregateId);
        return new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(1, string.Empty, string.Empty, string.Empty, string.Empty),
            scenario.Create<PersonCreated>().ToEnvelope(2, string.Empty, string.Empty, string.Empty, string.Empty),
        };
    }

    protected override void SeedToRebuild(OrganisationRegistryContext context, Guid aggregateId)
        => context.PeopleToRebuild.Add(new PersonToRebuild { PersonId = aggregateId });

    protected override async Task Verify(
        ElasticSearchFixture fixture, IContextFactory contextFactory, Guid aggregateId)
    {
        var document = fixture.Elastic.ReadClient.Get<PersonDocument>(aggregateId);
        document.Source.Should().NotBeNull();

        await using var ctx = contextFactory.Create();
        var remaining = await ctx.PeopleToRebuild.ToListAsync();
        remaining.Should().BeEmpty();
    }
}
