namespace OrganisationRegistry.ElasticSearch.Tests.IndividualRebuildRunners;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Projections;
using OrganisationRegistry.ElasticSearch.Client;
using OrganisationRegistry.ElasticSearch.Configuration;
using OrganisationRegistry.ElasticSearch.Projections.Infrastructure;
using OrganisationRegistry.ElasticSearch.Projections.People.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using Infrastructure.Events;
using SqlServer;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using OrganisationRegistry.Tests.Shared;
using OrganisationRegistry.Tests.Shared.Stubs;
using Projections.IndividualRebuild;
using Xunit;

/// <summary>
/// Base class for testing the three IndividualRebuildRunner variants.
/// Concrete tests supply the injected <see cref="IndividualRebuildRunnerConfig{TAggregate, TDocument, TToRebuild}"/>,
/// the runner instance to exercise, and the per-type test scaffolding (seed, envelopes, verify).
/// </summary>
public abstract class IndividualRebuildRunnerTestBase<TAggregate, TDocument, TToRebuild>
    where TDocument : class, IDocument
    where TToRebuild : class
{
    private readonly ElasticSearchFixture _fixture;

    protected IndividualRebuildRunnerTestBase(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    protected abstract IndividualRebuildRunnerConfig<TAggregate, TDocument, TToRebuild> Config { get; }

    protected abstract IndividualRebuildRunner<TAggregate, TDocument, TToRebuild> CreateRunner(
        IEventStore eventStore,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar);

    protected abstract List<IEnvelope> CreateEnvelopes(Guid aggregateId);
    protected abstract void SeedToRebuild(OrganisationRegistryContext context, Guid aggregateId);
    protected abstract Task Verify(
        ElasticSearchFixture fixture, IContextFactory contextFactory, Guid aggregateId);

    [Fact]
    public async Task Run_RebuildsDocumentAndRemovesPendingRow()
    {
        var aggregateId = Guid.NewGuid();
        var envelopes = CreateEnvelopes(aggregateId);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock
            .Setup(x => x.GetEventEnvelopesUntil<TAggregate>(aggregateId, It.IsAny<int>()))
            .Returns(envelopes);

        var projectionStatesMock = new Mock<IProjectionStates>();
        projectionStatesMock
            .Setup(x => x.GetLastProcessedEventNumber(It.IsAny<string>()))
            .ReturnsAsync(int.MaxValue);

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;
        var contextFactory = new TestContextFactory(dbContextOptions);

        await using (var seedContext = contextFactory.Create())
        {
            SeedToRebuild(seedContext, aggregateId);
            await seedContext.SaveChangesAsync();
        }

        var serviceProvider = BuildServiceProvider(contextFactory, _fixture, Config.EventHandlers);
        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        var busRegistrar = new ElasticBusRegistrar(
            new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);

        var runner = CreateRunner(
            eventStoreMock.Object, contextFactory, projectionStatesMock.Object, bus, _fixture.Elastic, busRegistrar);

        await runner.Run();

        await Verify(_fixture, contextFactory, aggregateId);
    }

    private static IServiceProvider BuildServiceProvider(
        IContextFactory contextFactory, ElasticSearchFixture fixture, Type[] eventHandlers)
    {
        var services = new ServiceCollection();

        foreach (var handlerType in eventHandlers)
        {
            var logger = Activator.CreateInstance(typeof(NullLogger<>).MakeGenericType(handlerType))!;
            var constructor = handlerType.GetConstructors().Single();
            var args = constructor.GetParameters().Select(p =>
            {
                if (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(ILogger<>))
                    return logger;
                if (p.ParameterType == typeof(Elastic))
                    return fixture.Elastic;
                if (p.ParameterType == typeof(IContextFactory))
                    return contextFactory;
                if (p.ParameterType == typeof(IOptions<ElasticSearchConfiguration>))
                    return fixture.ElasticSearchOptions;
                if (p.ParameterType == typeof(IEventStore))
                    return new Mock<IEventStore>().Object;
                if (p.ParameterType == typeof(IOrganisationManagementConfiguration))
                    return new OrganisationManagementConfigurationStub();
                if (p.ParameterType == typeof(IPersonHandlerCache))
                    return new PersonHandlerCacheStub();
                throw new InvalidOperationException($"Cannot resolve {p.ParameterType.Name} for {handlerType.Name}");
            }).ToArray();

            services.AddSingleton(handlerType, constructor.Invoke(args));
        }

        if (eventHandlers.Any(t => t.GetConstructors().Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(MemoryCachesMaintainer)))))
        {
            var memoryCaches = new MemoryCaches(contextFactory);
            services.AddSingleton(new MemoryCachesMaintainer(memoryCaches, contextFactory));
        }

        return services.BuildServiceProvider();
    }
}
