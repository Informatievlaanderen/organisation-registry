namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using People;
    using Person.Events;
    using Projections.Infrastructure;
    using Projections.Organisations;
    using Projections.People;
    using Projections.People.Cache;
    using Projections.People.Handlers;
    using Scenario;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using Xunit;

    [Collection(nameof(ElasticSearchFixture))]
    public class PersonHandlerTests
    {
        private readonly TestEventProcessor _eventProcessor;
        private readonly ElasticSearchFixture _fixture;
        private readonly TestContextFactory _testContextFactory;

        public PersonHandlerTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;

            var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;

            _testContextFactory = new TestContextFactory(dbContextOptions);

            var elastic = _fixture.Elastic;
            var elasticSearchOptions = _fixture.ElasticSearchOptions;

            var personHandlerCache = new PersonHandlerCacheStub();
            var personHandler = new Person(
                new NullLogger<Person>(),
                elastic,
                _testContextFactory,
                elasticSearchOptions,
                personHandlerCache);

            var personCapacity = new PersonCapacity(
                new NullLogger<PersonCapacity>(),
                _testContextFactory,
                personHandlerCache);
            var personFunction = new PersonFunction(
                new NullLogger<PersonFunction>(),
                _testContextFactory,
                personHandlerCache);
            var personMandate = new PersonMandate(
                new NullLogger<PersonMandate>(),
                _testContextFactory,
                personHandlerCache);

            var cachedOrganisationForBodies = new CachedOrganisationForBodies(
                new NullLogger<CachedOrganisationForBodies>(),
                _testContextFactory);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(personHandler)
                .AddSingleton(personCapacity)
                .AddSingleton(personFunction)
                .AddSingleton(personMandate)
                .AddSingleton(cachedOrganisationForBodies)
                .BuildServiceProvider();

            var bus = new ElasticBus(new NullLogger<ElasticBus>());
            _eventProcessor = new TestEventProcessor(bus, fixture);

            var registrar = new ElasticBusRegistrar(new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);
            registrar.RegisterEventHandlers(PeopleRunner.EventHandlers);
        }

        [EnvVarIgnoreFact]
        public async void InitializeProjection_CreatesIndex()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            await _eventProcessor.Handle<PersonDocument>(
                new List<IEnvelope>
                {
                    scenario.Create<InitialiseProjection>().ToEnvelope(),
                    scenario.Create<PersonCreated>().ToEnvelope()
                }
            );

            var indices =
                (await _fixture.Elastic.ReadClient.Indices.GetAsync(
                    _fixture.ElasticSearchOptions.Value.PeopleReadIndex)).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public async void PersonCreated_CreatesDocument()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var personCreated = scenario.Create<PersonCreated>();

            await _eventProcessor.Handle<PersonDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    personCreated.ToEnvelope()
                }
            );

            var person = _fixture.Elastic.ReadClient.Get<PersonDocument>(personCreated.PersonId);

            person.Source.Name.Should().Be(personCreated.Name);
        }

        [EnvVarIgnoreFact]
        public async void OrganisationTerminated()
        {
            var context = _testContextFactory.Create();
            var scenario = new PersonScenario(Guid.NewGuid());

            var contactTypeCacheItem = new ContactTypeCacheItem
            {
                Id = scenario.Create<Guid>(),
                Name = scenario.Create<string>()
            };
            context.ContactTypeCache.Add(contactTypeCacheItem);

            var organisationCacheItem = new OrganisationCacheItem
            {
                Id = scenario.Create<Guid>(),
                Name = scenario.Create<string>(),
                OvoNumber = scenario.Create<string>()
            };
            context.OrganisationCache.Add(organisationCacheItem);
            await context.SaveChangesAsync();

            var initialiseOrganisationProjection = new InitialiseProjection(typeof(Organisation).FullName!);
            var initialisePersonProjection = new InitialiseProjection(typeof(Person).FullName!);
            var personCreated = scenario.Create<PersonCreated>();
            var capacityAdded = scenario.CreateOrganisationCapacityAdded(
                personCreated.PersonId,
                organisationCacheItem.Id);
            var dateOfTermination = capacityAdded.ValidTo?.AddDays(-10) ??
                                    (scenario.Create<DateTime?>() ?? scenario.Create<DateTime>());
            var organisationTerminationCapacities = new Dictionary<Guid, DateTime>
            {
                { capacityAdded.OrganisationCapacityId, dateOfTermination }
            };

            var organisationTerminated = scenario.CreateOrganisationTerminated(
                capacityAdded.OrganisationId,
                dateOfTermination,
                capacities: organisationTerminationCapacities);

            await _eventProcessor.Handle<PersonDocument>(
                new List<IEnvelope>
                {
                    initialiseOrganisationProjection.ToEnvelope(),
                    initialisePersonProjection.ToEnvelope(),
                    personCreated.ToEnvelope(),
                    capacityAdded.ToEnvelope(),
                    organisationTerminated.ToEnvelope()
                }
            );

            var person = _fixture.Elastic.WriteClient.Get<PersonDocument>(personCreated.PersonId);

            person.Source.Name.Should().Be(personCreated.Name);
            person.Source.Capacities.Should().HaveCount(1);
            person.Source.Capacities.First().Validity.Start.Should().Be(capacityAdded.ValidFrom);
            person.Source.Capacities.First().Validity.End.Should().Be(dateOfTermination);
        }

        [EnvVarIgnoreFact]
        public async void OrganisationTerminatedV2()
        {
            var context = _testContextFactory.Create();
            var scenario = new PersonScenario(Guid.NewGuid());

            var contactTypeCacheItem = new ContactTypeCacheItem
            {
                Id = scenario.Create<Guid>(),
                Name = scenario.Create<string>()
            };
            context.ContactTypeCache.Add(contactTypeCacheItem);

            var organisationCacheItem = new OrganisationCacheItem
            {
                Id = scenario.Create<Guid>(),
                Name = scenario.Create<string>(),
                OvoNumber = scenario.Create<string>()
            };
            context.OrganisationCache.Add(organisationCacheItem);
            await context.SaveChangesAsync();

            var initialiseOrganisationProjection = new InitialiseProjection(typeof(Organisation).FullName!);
            var initialisePersonProjection = new InitialiseProjection(typeof(Person).FullName!);
            var personCreated = scenario.Create<PersonCreated>();
            var capacityAdded = scenario.CreateOrganisationCapacityAdded(
                personCreated.PersonId,
                organisationCacheItem.Id);
            var dateOfTermination = capacityAdded.ValidTo?.AddDays(-10) ??
                                    (scenario.Create<DateTime?>() ?? scenario.Create<DateTime>());
            var organisationTerminationCapacities = new Dictionary<Guid, DateTime>
            {
                { capacityAdded.OrganisationCapacityId, dateOfTermination }
            };

            var organisationTerminatedV2 = scenario.CreateOrganisationTerminatedV2(
                capacityAdded.OrganisationId,
                dateOfTermination,
                capacities: organisationTerminationCapacities);

            await _eventProcessor.Handle<PersonDocument>(
                new List<IEnvelope>
                {
                    initialiseOrganisationProjection.ToEnvelope(),
                    initialisePersonProjection.ToEnvelope(),
                    personCreated.ToEnvelope(),
                    capacityAdded.ToEnvelope(),
                    organisationTerminatedV2.ToEnvelope()
                }
            );

            var person = _fixture.Elastic.WriteClient.Get<PersonDocument>(personCreated.PersonId);

            person.Source.Name.Should().Be(personCreated.Name);
            person.Source.Capacities.Should().HaveCount(1);
            person.Source.Capacities.First().Validity.Start.Should().Be(capacityAdded.ValidFrom);
            person.Source.Capacities.First().Validity.End.Should().Be(dateOfTermination);
        }
    }
}
