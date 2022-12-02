namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bodies;
using FluentAssertions;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Cache;
using Infrastructure.Bus;
using Infrastructure.Config;
using Infrastructure.Configuration;
using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrganisationRegistry.Projections.Reporting;
using OrganisationRegistry.Tests.Shared;
using OrganisationRegistry.Tests.Shared.Stubs;
using Person;
using Person.Events;
using Projections.Body;
using Projections.Infrastructure;
using Scenario;
using SqlServer.Infrastructure;
using Xunit;

[Collection(nameof(ElasticSearchFixture))]
public class MEPCalulationServiceTests
{
    private readonly TestEventProcessor _eventProcessor;
    private readonly ElasticSearchFixture _fixture;
    private readonly InProcessBus _inProcessBus;

    public MEPCalulationServiceTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
        var bodyHandler = new BodyHandler(
            new NullLogger<BodyHandler>(),
            _fixture.Elastic,
            _fixture.ContextFactory,
            _fixture.ElasticSearchOptions,
            new MetricsBuilder().Build());

        var serviceProvider = new ServiceCollection()
            .AddSingleton(bodyHandler)
            .BuildServiceProvider();

        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        _eventProcessor = new TestEventProcessor(bus, fixture);

        var elasticBusRegistrar = new ElasticBusRegistrar(new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);
        elasticBusRegistrar.RegisterEventHandlers(BodyRunner.EventHandlers);

        var services = new ServiceCollection();

        var app = ConfigureServices(services, _fixture.Configuration, _fixture.ContextOptions);
        UseOrganisationRegistryEventSourcing(app);

        _inProcessBus = app.GetService<InProcessBus>()!;
    }

    private static IServiceProvider ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        DbContextOptions<OrganisationRegistryContext> dbContextOptions)
    {
        services.AddOptions();
        services.AddSingleton<ICache<OrganisationSecurityInformation>, OrganisationSecurityCache>()
            .AddSingleton<IOrganisationRegistryConfiguration>(
                new OrganisationRegistryConfigurationStub());

        var builder = new ContainerBuilder();
        builder.RegisterModule(new TestRunnerModule<OrganisationRegistryReportingRunnerAssemblyTokenClass>(configuration, services, new NullLoggerFactory(), dbContextOptions));

        return new AutofacServiceProvider(builder.Build());
    }

    private static void UseOrganisationRegistryEventSourcing(IServiceProvider app)
    {
        var registrar = app.GetService<BusRegistrar>()!;

        registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
        registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistryReportingRunnerAssemblyTokenClass));
    }

    protected async Task HandleEvents(params IEvent[] envelopes)
    {
        foreach (var envelope in envelopes) await _inProcessBus.Publish(Mock.Of<DbConnection>(), Mock.Of<DbTransaction>(), (dynamic)envelope.ToEnvelope());
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async void AssignedPersonToBodySeat_CalulatesMEPInformation(bool entitledToVote, bool isEffective)
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);
        var today = scenario.Create<DateTime>();

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var bodyRegistered = scenario.CreateBodyRegistered(bodyId);
        var bodySeatAdded = scenario.CreateBodySeatAdded(bodyId, scenario.Create<Guid>(), entitledToVote, isEffective);
        var assignedPersonToBodySeat = scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeatAdded.BodySeatId, scenario.Create<Guid>());

        var eventEnvelopes = new List<IEnvelope>
        {
            initialiseProjection.ToEnvelope(),
            bodyRegistered.ToEnvelope(),
            bodySeatAdded.ToEnvelope(),
            assignedPersonToBodySeat.ToEnvelope(),
        };

        await HandleEvents(eventEnvelopes.Select(ee => ee.Body).ToArray());
        await _eventProcessor.Handle<BodyDocument>(eventEnvelopes);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepCalculatorService = new TestableMEPCalculatorService(
            new NullLogger<TestableMEPCalculatorService>(),
            _fixture.Elastic,
            _fixture.ContextFactory,
            new DateTimeProviderStub(today));

        await mepCalculatorService.TestableProcessBodies(CancellationToken.None);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepInfo = _fixture.Elastic.ReadClient
            .Get<BodyDocument>(bodyId)
            .Source
            .MEP;

        mepInfo.EntitledToVote.Effective.AssignedSeatCount.Should().Be(entitledToVote && isEffective ? 1 : 0);
        mepInfo.EntitledToVote.Effective.TotalSeatCount.Should().Be(entitledToVote && isEffective ? 1 : 0);

        mepInfo.EntitledToVote.NotEffective.AssignedSeatCount.Should().Be(entitledToVote && !isEffective ? 1 : 0);
        mepInfo.EntitledToVote.NotEffective.TotalSeatCount.Should().Be(entitledToVote && !isEffective ? 1 : 0);

        mepInfo.EntitledToVote.Total.AssignedSeatCount.Should().Be(entitledToVote ? 1 : 0);
        mepInfo.EntitledToVote.Total.TotalSeatCount.Should().Be(entitledToVote ? 1 : 0);

        mepInfo.NotEntitledToVote.Effective.AssignedSeatCount.Should().Be(!entitledToVote && isEffective ? 1 : 0);
        mepInfo.NotEntitledToVote.Effective.TotalSeatCount.Should().Be(!entitledToVote && isEffective ? 1 : 0);

        mepInfo.NotEntitledToVote.NotEffective.AssignedSeatCount.Should().Be(!entitledToVote && !isEffective ? 1 : 0);
        mepInfo.NotEntitledToVote.NotEffective.TotalSeatCount.Should().Be(!entitledToVote && !isEffective ? 1 : 0);

        mepInfo.NotEntitledToVote.Total.AssignedSeatCount.Should().Be(!entitledToVote ? 1 : 0);
        mepInfo.NotEntitledToVote.Total.TotalSeatCount.Should().Be(!entitledToVote ? 1 : 0);

        mepInfo.Total.Effective.AssignedSeatCount.Should().Be(isEffective ? 1 : 0);
        mepInfo.Total.Effective.TotalSeatCount.Should().Be(isEffective ? 1 : 0);

        mepInfo.Total.NotEffective.AssignedSeatCount.Should().Be(!isEffective ? 1 : 0);
        mepInfo.Total.NotEffective.TotalSeatCount.Should().Be(!isEffective ? 1 : 0);

        mepInfo.Total.Total.AssignedSeatCount.Should().Be(expected: 1);
        mepInfo.Total.Total.TotalSeatCount.Should().Be(expected: 1);
    }

    [Theory]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NoSeats, MEPCompliance.NoSeats, MEPCompliance.NoSeats)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NoSeats, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.Compliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NoSeats, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    public async void MEPComplianceNotEntitledToVoteTheory(
        MEPCompliance notEntitledToVoteEffectiveMEPCompliance,
        MEPCompliance notEntitledToVoteNotEffectiveMEPCompliance,
        MEPCompliance expectedNotEntitledToVoteMEPCompliance,
        MEPCompliance expectedTotalCompliance)
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);
        var today = scenario.Create<DateTime>();

        var eventEnvelopes = new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(),
            scenario.CreateBodyRegistered(bodyId).ToEnvelope(),
        };

        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: false, isEffective: true, notEntitledToVoteEffectiveMEPCompliance));
        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: false, isEffective: false, notEntitledToVoteNotEffectiveMEPCompliance));

        await HandleEvents(eventEnvelopes.Select(ee => ee.Body).ToArray());
        await _eventProcessor.Handle<BodyDocument>(eventEnvelopes);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepCalculatorService = new TestableMEPCalculatorService(
            new NullLogger<TestableMEPCalculatorService>(),
            _fixture.Elastic,
            _fixture.ContextFactory,
            new DateTimeProviderStub(today));

        await mepCalculatorService.TestableProcessBodies(CancellationToken.None);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepInfo = _fixture.Elastic.ReadClient
            .Get<BodyDocument>(bodyId)
            .Source
            .MEP;

        mepInfo.NotEntitledToVote.Effective.MEPCompliance.Should().Be(notEntitledToVoteEffectiveMEPCompliance);
        mepInfo.NotEntitledToVote.NotEffective.MEPCompliance.Should().Be(notEntitledToVoteNotEffectiveMEPCompliance);
        mepInfo.NotEntitledToVote.Total.MEPCompliance.Should().Be(expectedNotEntitledToVoteMEPCompliance);
        mepInfo.Total.Total.MEPCompliance.Should().Be(expectedTotalCompliance);
    }

    [Theory]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NoSeats, MEPCompliance.NoSeats, MEPCompliance.NoSeats)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NoSeats, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.Compliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NoSeats, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    public async void MEPComplianceEntitledToVoteTheory(
        MEPCompliance entitledToVoteEffectiveMEPCompliance,
        MEPCompliance entitledToVoteNotEffectiveMEPCompliance,
        MEPCompliance expectedEntitledToVoteMEPCompliance,
        MEPCompliance expectedTotalCompliance)
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);
        var today = scenario.Create<DateTime>();

        var eventEnvelopes = new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(),
            scenario.CreateBodyRegistered(bodyId).ToEnvelope(),
        };

        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: true, isEffective: true, entitledToVoteEffectiveMEPCompliance));
        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: true, isEffective: false, entitledToVoteNotEffectiveMEPCompliance));

        await HandleEvents(eventEnvelopes.Select(ee => ee.Body).ToArray());
        await _eventProcessor.Handle<BodyDocument>(eventEnvelopes);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepCalculatorService = new TestableMEPCalculatorService(
            new NullLogger<TestableMEPCalculatorService>(),
            _fixture.Elastic,
            _fixture.ContextFactory,
            new DateTimeProviderStub(today));

        await mepCalculatorService.TestableProcessBodies(CancellationToken.None);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepInfo = _fixture.Elastic.ReadClient
            .Get<BodyDocument>(bodyId)
            .Source
            .MEP;

        mepInfo.EntitledToVote.Effective.MEPCompliance.Should().Be(entitledToVoteEffectiveMEPCompliance);
        mepInfo.EntitledToVote.NotEffective.MEPCompliance.Should().Be(entitledToVoteNotEffectiveMEPCompliance);
        mepInfo.EntitledToVote.Total.MEPCompliance.Should().Be(expectedEntitledToVoteMEPCompliance);
        mepInfo.Total.Total.MEPCompliance.Should().Be(expectedTotalCompliance);
    }

    [Theory]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NoSeats, MEPCompliance.NoSeats)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NoSeats, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NoSeats, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotAllSeatsAssigned, MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NoSeats, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.NotCompliant, MEPCompliance.Compliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NoSeats, MEPCompliance.Compliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotAllSeatsAssigned, MEPCompliance.NotAllSeatsAssigned)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.NotCompliant, MEPCompliance.NotCompliant)]
    [InlineData(MEPCompliance.Compliant, MEPCompliance.Compliant, MEPCompliance.Compliant)]
    public async void MEPComplianceTotalTheory(
        MEPCompliance totalEffectiveMEPCompliance,
        MEPCompliance totalNotEffectiveMEPCompliance,
        MEPCompliance expectedTotalCompliance)
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);
        var today = scenario.Create<DateTime>();

        var eventEnvelopes = new List<IEnvelope>
        {
            scenario.Create<InitialiseProjection>().ToEnvelope(),
            scenario.CreateBodyRegistered(bodyId).ToEnvelope(),
        };

        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: scenario.Create<bool>(), isEffective: true, totalEffectiveMEPCompliance));
        eventEnvelopes.AddRange(CreateEventsForMEPCompliance(scenario, bodyId, entitledToVote: scenario.Create<bool>(), isEffective: false, totalNotEffectiveMEPCompliance));

        await HandleEvents(eventEnvelopes.Select(ee => ee.Body).ToArray());
        await _eventProcessor.Handle<BodyDocument>(eventEnvelopes);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepCalculatorService = new TestableMEPCalculatorService(
            new NullLogger<TestableMEPCalculatorService>(),
            _fixture.Elastic,
            _fixture.ContextFactory,
            new DateTimeProviderStub(today));

        await mepCalculatorService.TestableProcessBodies(CancellationToken.None);

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync();

        var mepInfo = _fixture.Elastic.ReadClient
            .Get<BodyDocument>(bodyId)
            .Source
            .MEP;

        mepInfo.Total.Effective.MEPCompliance.Should().Be(totalEffectiveMEPCompliance);
        mepInfo.Total.NotEffective.MEPCompliance.Should().Be(totalNotEffectiveMEPCompliance);
        mepInfo.Total.Total.MEPCompliance.Should().Be(expectedTotalCompliance);
    }

    private static List<IEnvelope> CreateEventsForMEPCompliance(BodyScenario scenario, Guid bodyId, bool entitledToVote, bool isEffective, MEPCompliance mepCompliance)
    {
        var eventEnvelopes = new List<IEnvelope>();

        switch (mepCompliance)
        {
            case MEPCompliance.Compliant:
            {
                var malePersonEffectiveEvents = CreatePersonAndAssignToSeats(scenario, bodyId, 2, Sex.Male, entitledToVote, isEffective);
                eventEnvelopes.AddRange(malePersonEffectiveEvents.Select(e => e.ToEnvelope()));

                var femalePersonEffectiveEvents = CreatePersonAndAssignToSeats(scenario, bodyId, 2, Sex.Female, entitledToVote, isEffective);
                eventEnvelopes.AddRange(femalePersonEffectiveEvents.Select(e => e.ToEnvelope()));

                break;
            }
            case MEPCompliance.NotCompliant:
            {
                var malePersonEffectiveEvents = CreatePersonAndAssignToSeats(scenario, bodyId, 2, Sex.Male, entitledToVote, isEffective);
                eventEnvelopes.AddRange(malePersonEffectiveEvents.Select(e => e.ToEnvelope()));

                break;
            }
            case MEPCompliance.NotAllSeatsAssigned:
            {
                var malePersonEffectiveEvents = CreatePersonAndAssignToSeats(scenario, bodyId, 2, Sex.Male, entitledToVote, isEffective);
                eventEnvelopes.AddRange(malePersonEffectiveEvents.Select(e => e.ToEnvelope()));

                var bodySeatAdded = scenario.CreateBodySeatAdded(bodyId, scenario.Create<Guid>(), entitledToVote, isEffective);
                eventEnvelopes.Add(bodySeatAdded.ToEnvelope());

                break;
            }
            case MEPCompliance.NoSeats:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mepCompliance), mepCompliance, null);
        }

        return eventEnvelopes;
    }

    private static IEnumerable<IEvent> CreatePersonAndAssignToSeats(
        BodyScenario scenario,
        Guid bodyId,
        int numberOfSeats,
        Sex personSex,
        bool entitledToVote,
        bool isEffective)
    {
        var person = new PersonCreated(Guid.NewGuid(), string.Empty, string.Empty, personSex, DateTime.Now);
        yield return person;

        for (var i = 0; i < numberOfSeats; i++)
        {
            var bodySeatAdded = scenario.CreateBodySeatAdded(bodyId, scenario.Create<Guid>(), entitledToVote, isEffective);
            yield return bodySeatAdded;
            yield return scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeatAdded.BodySeatId, person.PersonId);
        }
    }
}
