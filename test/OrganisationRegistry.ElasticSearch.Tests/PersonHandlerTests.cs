namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.AppSpecific;
    using Infrastructure.Bus;
    using Infrastructure.Config;
    using Infrastructure.Events;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Organisation.Events;
    using People;
    using Person.Events;
    using Projections;
    using Projections.Infrastructure;
    using Projections.People.Handlers;
    using Scenario;
    using SqlServer.Infrastructure;
    using Xunit;

    [Collection(nameof(ElasticSearchFixture))]
    public class PersonHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly InProcessBus _inProcessBus;

        public PersonHandlerTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;

            var memoryCaches = new Mock<IMemoryCaches>();
            memoryCaches.Setup(caches => caches.OrganisationNames[It.IsAny<Guid>()]).Returns("org name");
            memoryCaches.Setup(caches => caches.ContactTypeNames[It.IsAny<Guid>()]).Returns("contact type name");

            var personHandler = new Person(
                logger: _fixture.LoggerFactory.CreateLogger<Person>(),
                elastic: _fixture.Elastic,
                contextFactory: _fixture.ContextFactory,
                elasticSearchOptions: _fixture.ElasticSearchOptions);

            var personCapacityHandler = new PersonCapacity(
                logger: _fixture.LoggerFactory.CreateLogger<PersonCapacity>(),
                elastic: _fixture.Elastic,
                contextFactory: _fixture.ContextFactory,
                memoryCaches.Object);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(personHandler)
                .AddSingleton(personCapacityHandler)
                .AddSingleton(new MemoryCachesMaintainer(new MemoryCaches(fixture.ContextFactory), fixture.ContextFactory))
                .BuildServiceProvider();

            _inProcessBus = new InProcessBus(new NullLogger<InProcessBus>());
            var registrar = new BusRegistrar(new NullLogger<BusRegistrar>(), _inProcessBus, () => serviceProvider);
            registrar.RegisterEventHandlers(PeopleRunner.EventHandlers);
        }

        [EnvVarIgnoreFact]
        public void InitializeProjection_CreatesIndex()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            Handle(scenario.Create<InitialiseProjection>());

            var indices = _fixture.Elastic.ReadClient.Indices.Get(_fixture.ElasticSearchOptions.Value.PeopleReadIndex).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public void PersonCreated_CreatesDocument()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var personCreated = scenario.Create<PersonCreated>();

            Handle(
                initialiseProjection,
                personCreated);

            var person = _fixture.Elastic.ReadClient.Get<PersonDocument>(personCreated.PersonId);

            person.Source.Name.Should().Be(personCreated.Name);
        }

        [EnvVarIgnoreFact]
        public void OrganisationTerminated()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var personCreated = scenario.Create<PersonCreated>();
            var capacityAdded = scenario.Create<OrganisationCapacityAdded>();
            var dateOfTermination = scenario.Create<DateTime>().Date;
            var organisationTerminationCapacities = new Dictionary<Guid, DateTime>
            {
                {capacityAdded.CapacityId, dateOfTermination}
            };
            var organisationTerminated = new OrganisationTerminated(
                organisationId: capacityAdded.OrganisationId,
                name: scenario.Create<string>(),
                ovoNumber: scenario.Create<string>(),
                dateOfTermination: dateOfTermination,
                new FieldsToTerminate(
                    organisationNewValidTo: scenario.Create<DateTime>(),
                    buildingsToTerminate: new Dictionary<Guid, DateTime>(),
                    bankAccountsToTerminate: organisationTerminationCapacities,
                    capacitiesToTerminate: new Dictionary<Guid, DateTime>(),
                    contactsToTerminate: new Dictionary<Guid, DateTime>(),
                    classificationsToTerminate: new Dictionary<Guid, DateTime>(),
                    functionsToTerminate: new Dictionary<Guid, DateTime>(),
                    labelsToTerminate: new Dictionary<Guid, DateTime>(),
                    locationsToTerminate: new Dictionary<Guid, DateTime>(),
                    parentsToTerminate: new Dictionary<Guid, DateTime>(),
                    relationsToTerminate: new Dictionary<Guid, DateTime>(),
                    formalFrameworksToTerminate: new Dictionary<Guid, DateTime>(),
                    openingHoursToTerminate: new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminate(
                    kboBankAccountsToTerminate: new Dictionary<Guid, DateTime>(),
                    kboRegisteredOfficeToTerminate: null,
                    kboFormalNameToTerminate: null,
                    kboLegalFormToTerminate: null
                ),
                forcedKboTermination: scenario.Create<bool>()
            );

            Handle(
                initialiseProjection,
                personCreated,
                capacityAdded,
                organisationTerminated);

            var person = _fixture.Elastic.ReadClient.Get<PersonDocument>(personCreated.PersonId);

            person.Source.Name.Should().Be(personCreated.Name);
            person.Source.Capacities.Should().HaveCount(1);
            person.Source.Capacities.First().Validity.Start.Should().Be(capacityAdded.ValidFrom);
            person.Source.Capacities.First().Validity.End.Should().Be(dateOfTermination);
        }

        private void Handle(params IEvent[] envelopes)
        {
            foreach (var envelope in envelopes)
            {
                _inProcessBus.Publish(null, null, (dynamic)envelope.ToEnvelope());
            }
        }
    }
}
