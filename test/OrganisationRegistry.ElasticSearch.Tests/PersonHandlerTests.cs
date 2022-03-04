namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api.Security;
    using FluentAssertions;
    using Infrastructure.Bus;
    using Infrastructure.Config;
    using Infrastructure.Events;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Organisation.Events;
    using OrganisationRegistry.Tests.Shared.Stubs;
    using People;
    using Person.Events;
    using Projections.Infrastructure;
    using Projections.People;
    using Projections.People.Handlers;
    using Scenario;
    using Security;
    using Xunit;

    [Collection(nameof(ElasticSearchFixture))]
    public class PersonHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly InProcessBus _inProcessBus;

        public PersonHandlerTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;

            var personHandler = new Person(
                _fixture.LoggerFactory.CreateLogger<Person>(),
                _fixture.Elastic,
                _fixture.ContextFactory,
                _fixture.ElasticSearchOptions);

            var personCapacityHandler = new PersonCapacity(
                _fixture.LoggerFactory.CreateLogger<PersonCapacity>(),
                _fixture.ContextFactory);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(personHandler)
                .AddSingleton(personCapacityHandler)
                .BuildServiceProvider();

            _inProcessBus = new InProcessBus(
                new NullLogger<InProcessBus>(),
                new SecurityService(
                    fixture.ContextFactory,
                    new OrganisationRegistryConfigurationStub(),
                    Mock.Of<ICache<OrganisationSecurityInformation>>()));
            var registrar = new BusRegistrar(new NullLogger<BusRegistrar>(), _inProcessBus, () => serviceProvider);
            registrar.RegisterEventHandlers(PeopleRunner.EventHandlers);
        }

        [EnvVarIgnoreFact]
        public void InitializeProjection_CreatesIndex()
        {
            var scenario = new PersonScenario(Guid.NewGuid());

            Handle(scenario.Create<InitialiseProjection>());

            var indices = _fixture.Elastic.ReadClient.Indices.Get(_fixture.ElasticSearchOptions.Value.PeopleReadIndex)
                .Indices;
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
                { capacityAdded.CapacityId, dateOfTermination }
            };
            var organisationTerminated = new OrganisationTerminated(
                capacityAdded.OrganisationId,
                scenario.Create<string>(),
                scenario.Create<string>(),
                dateOfTermination,
                new FieldsToTerminate(
                    scenario.Create<DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    organisationTerminationCapacities,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminate(
                    new Dictionary<Guid, DateTime>(),
                    null,
                    null,
                    null
                ),
                scenario.Create<bool>()
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
            foreach (var envelope in envelopes) _inProcessBus.Publish(null, null, (dynamic)envelope.ToEnvelope());
        }
    }
}
