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

    [Fact]
    public async void Given_EntitledToVote_Effective_And_NotEffective_Are_Both_Not_MEPCompliant_Then_Entitled_To_Vote_Is_Not_MEPCompliant_And_BodyParticipation_Is_Not_MEPCompliant()
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);
        var today = scenario.Create<DateTime>();

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var bodyRegistered = scenario.CreateBodyRegistered(bodyId);

        var malePerson = new PersonCreated(Guid.NewGuid(), string.Empty, string.Empty, Sex.Male, DateTime.Now);
        var bodySeatAdded = scenario.CreateBodySeatAdded(bodyId, entitledToVote: true, isEffective: true);
        var bodySeat2Added = scenario.CreateBodySeatAdded(bodyId, entitledToVote: true, isEffective: true);
        var assignedMalePersonToBodySeat = scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeatAdded.BodySeatId, malePerson.PersonId);
        var assignedMalePersonToBodySeat2 = scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeat2Added.BodySeatId, malePerson.PersonId);

        var femalePerson = new PersonCreated(Guid.NewGuid(), string.Empty, string.Empty, Sex.Female, DateTime.Now);
        var bodySeat3Added = scenario.CreateBodySeatAdded(bodyId, entitledToVote: true, isEffective: false);
        var bodySeat4Added = scenario.CreateBodySeatAdded(bodyId, entitledToVote: true, isEffective: false);
        var assignedFemalePersonToBodySeat3 = scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeat3Added.BodySeatId, femalePerson.PersonId);
        var assignedFemalePersonToBodySeat4 = scenario.CreateAssignedPersonToBodySeat(bodyId, bodySeat4Added.BodySeatId, femalePerson.PersonId);

        var eventEnvelopes = new List<IEnvelope>
        {
            initialiseProjection.ToEnvelope(),
            bodyRegistered.ToEnvelope(),
            bodySeatAdded.ToEnvelope(),
            bodySeat2Added.ToEnvelope(),
            malePerson.ToEnvelope(),
            femalePerson.ToEnvelope(),
            assignedMalePersonToBodySeat.ToEnvelope(),
            assignedMalePersonToBodySeat2.ToEnvelope(),
            bodySeat3Added.ToEnvelope(),
            bodySeat4Added.ToEnvelope(),
            assignedFemalePersonToBodySeat3.ToEnvelope(),
            assignedFemalePersonToBodySeat4.ToEnvelope(),
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

        mepInfo.EntitledToVote.Total.IsMEPCompliant.Should().BeFalse();
        mepInfo.Total.Total.IsMEPCompliant.Should().BeFalse();
    }
}
