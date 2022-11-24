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
using Projections.Body;
using Projections.Infrastructure;
using Scenario;
using SqlServer.Infrastructure;
using Xunit;

[Collection(nameof(ElasticSearchFixture))]
public class MEPCalulationServiceTests
{
    private readonly ElasticSearchFixture _fixture;
    private readonly TestEventProcessor _eventProcessor;
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
        foreach (var envelope in envelopes)
        {
            await _inProcessBus.Publish(Mock.Of<DbConnection>(), Mock.Of<DbTransaction>(), (dynamic)envelope.ToEnvelope());
        }
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
        var bodySeatAdded = scenario.CreateBodySeatAdded(bodyId, entitledToVote, isEffective);
        var assignedPersonToBodySeat = scenario.CreateAssignedPersonToBodySeat(bodyId);

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

        mepInfo.Stemgerechtigd.Effectief.AantalToegewezenPosten.Should().Be(entitledToVote && isEffective ? 1 : 0);
        mepInfo.Stemgerechtigd.Effectief.AantalPosten.Should().Be(entitledToVote && isEffective ? 1 : 0);

        mepInfo.Stemgerechtigd.NietEffectief.AantalToegewezenPosten.Should().Be(entitledToVote && !isEffective ? 1 : 0);
        mepInfo.Stemgerechtigd.NietEffectief.AantalPosten.Should().Be(entitledToVote && !isEffective ? 1 : 0);

        mepInfo.Stemgerechtigd.Totaal.AantalToegewezenPosten.Should().Be(entitledToVote ? 1 : 0);
        mepInfo.Stemgerechtigd.Totaal.AantalPosten.Should().Be(entitledToVote ? 1 : 0);

        mepInfo.NietStemgerechtigd.Effectief.AantalToegewezenPosten.Should().Be(!entitledToVote && isEffective ? 1 : 0);
        mepInfo.NietStemgerechtigd.Effectief.AantalPosten.Should().Be(!entitledToVote && isEffective ? 1 : 0);

        mepInfo.NietStemgerechtigd.NietEffectief.AantalToegewezenPosten.Should().Be(!entitledToVote && !isEffective ? 1 : 0);
        mepInfo.NietStemgerechtigd.NietEffectief.AantalPosten.Should().Be(!entitledToVote && !isEffective ? 1 : 0);

        mepInfo.NietStemgerechtigd.Totaal.AantalToegewezenPosten.Should().Be(!entitledToVote ? 1 : 0);
        mepInfo.NietStemgerechtigd.Totaal.AantalPosten.Should().Be(!entitledToVote ? 1 : 0);

        mepInfo.Totaal.Effectief.AantalToegewezenPosten.Should().Be(isEffective ? 1 : 0);
        mepInfo.Totaal.Effectief.AantalPosten.Should().Be(isEffective ? 1 : 0);

        mepInfo.Totaal.NietEffectief.AantalToegewezenPosten.Should().Be(!isEffective ? 1 : 0);
        mepInfo.Totaal.NietEffectief.AantalPosten.Should().Be(!isEffective ? 1 : 0);

        mepInfo.Totaal.Totaal.AantalToegewezenPosten.Should().Be(1);
        mepInfo.Totaal.Totaal.AantalPosten.Should().Be(1);
    }
}
