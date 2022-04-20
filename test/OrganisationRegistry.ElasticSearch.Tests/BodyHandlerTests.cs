namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System.Linq;
    using Bodies;
    using Body.Events;
    using FluentAssertions;
    using Infrastructure.Events;
    using LifecyclePhaseType.Events;
    using Projections.Body;
    using Projections.Infrastructure;
    using Scenario;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using App.Metrics;
    using Scenario.Specimen;
    using Function.Events;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using Organisation.Events;
    using Person.Events;
    using SeatType.Events;

    [Collection(nameof(ElasticSearchFixture))]
    public class BodyHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly TestEventProcessor _eventProcessor;

        public BodyHandlerTests(ElasticSearchFixture fixture)
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

            var registrar = new ElasticBusRegistrar(new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);
            registrar.RegisterEventHandlers(BodyRunner.EventHandlers);
        }

        [EnvVarIgnoreFact]
        public async void InitializeProjection_CreatesIndex()
        {
            var scenario = new BodyScenario(Guid.NewGuid());

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    scenario.Create<InitialiseProjection>().ToEnvelope(),
                }
            );

            var indices = _fixture.Elastic.ReadClient.Indices.Get(_fixture.ElasticSearchOptions.Value.BodyWriteIndex).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public async void BodyRegistered_CreatesDocument()
        {
            var scenario = new BodyScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                }
            );

            var bodyDocument = _fixture.Elastic.ReadClient.Get<BodyDocument>(bodyRegistered.BodyId);

            bodyDocument.Source.Name.Should().Be(bodyRegistered.Name);
            bodyDocument.Source.BodyNumber.Should().Be(bodyRegistered.BodyNumber);
            bodyDocument.Source.ShortName.Should().Be(bodyRegistered.ShortName);
            bodyDocument.Source.Description.Should().Be(bodyRegistered.Description);
            bodyDocument.Source.FormalValidity.Start.Should().Be(bodyRegistered.FormalValidFrom);
            bodyDocument.Source.FormalValidity.End.Should().Be(bodyRegistered.FormalValidTo);
            bodyDocument.Source.LifecyclePhases.Count.Should().Be(0, "Lifecycle phases are added by a different event.");
        }

        [EnvVarIgnoreFact]
        public async void LifecyclePhaseTypeUpdated_UpdatesExistingBodyLifecyclePhases()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodyLifecyclePhaseAdded = scenario.Create<BodyLifecyclePhaseAdded>();
            var lifecyclePhaseTypeUpdated = scenario.Create<LifecyclePhaseTypeUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodyLifecyclePhaseAdded.ToEnvelope(),
                    lifecyclePhaseTypeUpdated.ToEnvelope(),
                }
            );

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .LifecyclePhases
                .Single()
                .LifecyclePhaseTypeName
                .Should().Be(lifecyclePhaseTypeUpdated.Name);
        }

        [EnvVarIgnoreFact]
        public async void MultipleBodySeatAddeds_CreatesMultipleBodySeats()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase<BodyHandler>(
                new ParameterNameArg<Guid>("bodyId", bodyId));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var anotherBodySeatAdded = scenario.Create<BodySeatAdded>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    anotherBodySeatAdded.ToEnvelope(),
                }
            );

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Count.Should().Be(2);
        }

        [EnvVarIgnoreFact]
        public async void AssignedPersonToBodySeat_AddsMandateToBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedPersonToBodySeat.ToEnvelope(),
                }
            );

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(assignedPersonToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(null);
            bodyMandate.OrganisationName.Should().Be(string.Empty);
            bodyMandate.FunctionTypeId.Should().Be(null);
            bodyMandate.FunctionTypeName.Should().Be(string.Empty);
            bodyMandate.PersonId.Should().Be(assignedPersonToBodySeat.PersonId);
            bodyMandate.PersonName.Should().Be(assignedPersonToBodySeat.PersonFirstName + " " + assignedPersonToBodySeat.PersonName);
            bodyMandate.Validity.Start.Should().Be(assignedPersonToBodySeat.ValidFrom);
            bodyMandate.Validity.End.Should().Be(assignedPersonToBodySeat.ValidTo);
        }

        [EnvVarIgnoreFact]
        public async void ReassignedPersonToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var reassignedPersonToBodySeat = scenario.Create<ReassignedPersonToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedPersonToBodySeat.ToEnvelope(),
                    reassignedPersonToBodySeat.ToEnvelope(),
                }
            );

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(reassignedPersonToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(null);
            bodyMandate.OrganisationName.Should().Be(string.Empty);
            bodyMandate.FunctionTypeId.Should().Be(null);
            bodyMandate.FunctionTypeName.Should().Be(string.Empty);
            bodyMandate.PersonId.Should().Be(reassignedPersonToBodySeat.PersonId);
            bodyMandate.PersonName.Should().Be(reassignedPersonToBodySeat.PersonFirstName + " " + reassignedPersonToBodySeat.PersonName);
            bodyMandate.Validity.Start.Should().Be(reassignedPersonToBodySeat.ValidFrom);
            bodyMandate.Validity.End.Should().Be(reassignedPersonToBodySeat.ValidTo);
        }

        [EnvVarIgnoreFact]
        public async void ReassignedPersonToBodySeat_DoesNotUpdateOtherMandates()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase<BodyHandler>(
                new ParameterNameArg<Guid>("bodyId", bodyId),
                new ParameterNameArg<Guid>("lifecyclePhaseTypeId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("bodySeatId", Guid.NewGuid()));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var assignedOtherPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            scenario.AddCustomization(new ParameterNameArg<Guid>("bodyMandateId", assignedPersonToBodySeat.BodyMandateId));
            var reassignedPersonToBodySeat = scenario.Create<ReassignedPersonToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedPersonToBodySeat.ToEnvelope(),
                    assignedOtherPersonToBodySeat.ToEnvelope(),
                    reassignedPersonToBodySeat.ToEnvelope(),
                }
            );

            var bodySeat = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single();

            bodySeat.Mandates.Count.Should().Be(2);

            var otherBodyMandate = bodySeat
                .Mandates
                .Single(mandate => mandate.BodyMandateId == assignedOtherPersonToBodySeat.BodyMandateId);

            otherBodyMandate.BodyMandateId.Should().Be(assignedOtherPersonToBodySeat.BodyMandateId);
            otherBodyMandate.OrganisationId.Should().Be(null);
            otherBodyMandate.OrganisationName.Should().Be(string.Empty);
            otherBodyMandate.FunctionTypeId.Should().Be(null);
            otherBodyMandate.FunctionTypeName.Should().Be(string.Empty);
            otherBodyMandate.PersonId.Should().Be(assignedOtherPersonToBodySeat.PersonId);
            otherBodyMandate.PersonName.Should().Be(assignedOtherPersonToBodySeat.PersonFirstName + " " + assignedOtherPersonToBodySeat.PersonName);
            otherBodyMandate.Validity.Start.Should().Be(assignedOtherPersonToBodySeat.ValidFrom);
            otherBodyMandate.Validity.End.Should().Be(assignedOtherPersonToBodySeat.ValidTo);
        }

        [EnvVarIgnoreFact]
        public async void MultipleAssignPersonToBodySeat_CreatesMultipleBodyMandates()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase<BodyHandler>(
                new ParameterNameArg<Guid>("bodyId", bodyId),
                new ParameterNameArg<Guid>("bodySeatId", Guid.NewGuid()));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var assignedOtherPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedPersonToBodySeat.ToEnvelope(),
                    assignedOtherPersonToBodySeat.ToEnvelope(),
                }
            );

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Count.Should().Be(2);
        }

        [EnvVarIgnoreFact]
        public async void ReassignedOrganisationToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var reassignedOrganisationToBodySeat = scenario.Create<ReassignedOrganisationToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    reassignedOrganisationToBodySeat.ToEnvelope(),
                }
            );

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(reassignedOrganisationToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(reassignedOrganisationToBodySeat.OrganisationId);
            bodyMandate.OrganisationName.Should().Be(reassignedOrganisationToBodySeat.OrganisationName);
            bodyMandate.FunctionTypeId.Should().Be(null);
            bodyMandate.FunctionTypeName.Should().Be(string.Empty);
            bodyMandate.PersonId.Should().Be(null);
            bodyMandate.PersonName.Should().Be(string.Empty);
            bodyMandate.Validity.Start.Should().Be(reassignedOrganisationToBodySeat.ValidFrom);
            bodyMandate.Validity.End.Should().Be(reassignedOrganisationToBodySeat.ValidTo);
        }

        [EnvVarIgnoreFact]
        public async void ReassignedFunctionTypeToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedFunctionTypeToBodySeat = scenario.Create<AssignedFunctionTypeToBodySeat>();
            var reassignedFunctionToBodySeat = scenario.Create<ReassignedFunctionTypeToBodySeat>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedFunctionTypeToBodySeat.ToEnvelope(),
                    reassignedFunctionToBodySeat.ToEnvelope(),
                }
            );

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(reassignedFunctionToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(reassignedFunctionToBodySeat.OrganisationId);
            bodyMandate.OrganisationName.Should().Be(reassignedFunctionToBodySeat.OrganisationName);
            bodyMandate.FunctionTypeId.Should().Be(reassignedFunctionToBodySeat.FunctionTypeId);
            bodyMandate.FunctionTypeName.Should().Be(reassignedFunctionToBodySeat.FunctionTypeName);
            bodyMandate.PersonId.Should().Be(null);
            bodyMandate.PersonName.Should().Be(string.Empty);
            bodyMandate.Validity.Start.Should().Be(reassignedFunctionToBodySeat.ValidFrom);
            bodyMandate.Validity.End.Should().Be(reassignedFunctionToBodySeat.ValidTo);
        }

        [EnvVarIgnoreFact]
        public async void PersonAssignedToDelegation_AddsDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.CreatePersonAssignedToDelegation(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId);

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    personAssignedToDelegation.ToEnvelope(),
                }
            );

            var bodySeat = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single();
            var bodyMandate = bodySeat
                .Mandates
                .Single();
            var delegation = bodyMandate
                .Delegations
                .Single();

            delegation.DelegationAssignmentId.Should().Be(personAssignedToDelegation.DelegationAssignmentId);
            delegation.PersonId.Should().Be(personAssignedToDelegation.PersonId);
            delegation.PersonName.Should().Be(personAssignedToDelegation.PersonFullName);
            delegation.Contacts.Should().HaveCount(personAssignedToDelegation.Contacts.Count);

            foreach (var (key, value) in personAssignedToDelegation.Contacts)
            {
                var delegationContact = delegation.Contacts.Single(x => x.ContactTypeId == key);
                delegationContact.Value.Should().Be(value);
            }
        }

        [EnvVarIgnoreFact]
        public async void PersonAssignedToDelegationUpdated_UpdatesDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.CreatePersonAssignedToDelegation(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId);
            var personAssignedToDelegationUpdated = scenario.CreatePersonAssignedToDelegationUpdated(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId,
                personAssignedToDelegation.DelegationAssignmentId);

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    personAssignedToDelegation.ToEnvelope(),
                    personAssignedToDelegationUpdated.ToEnvelope(),
                }
            );

            var bodySeat = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source.Seats.Single();
            var bodyMandate = bodySeat
                .Mandates.Single();
            var delegation = bodyMandate
                .Delegations.Single();

            delegation.DelegationAssignmentId.Should().Be(personAssignedToDelegationUpdated.DelegationAssignmentId);
            delegation.PersonId.Should().Be(personAssignedToDelegationUpdated.PersonId);
            delegation.PersonName.Should().Be(personAssignedToDelegationUpdated.PersonFullName);
            delegation.Contacts.Should().HaveCount(personAssignedToDelegationUpdated.Contacts.Count);

            foreach (var (key, value) in personAssignedToDelegationUpdated.Contacts)
            {
                var delegationContact = delegation.Contacts.Single(x => x.ContactTypeId == key);
                delegationContact.Value.Should().Be(value);
            }
        }

        [EnvVarIgnoreFact]
        public async void PersonAssignedToDelegationRemoved_RemovesDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.CreatePersonAssignedToDelegation(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId);
            var personAssignedToDelegationUpdated = scenario.CreatePersonAssignedToDelegationUpdated(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId,
                personAssignedToDelegation.DelegationAssignmentId);
            var personAssignedToDelegationRemoved = scenario.CreatePersonAssignedToDelegationRemoved(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId,
                personAssignedToDelegation.DelegationAssignmentId);

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    personAssignedToDelegation.ToEnvelope(),
                    personAssignedToDelegationUpdated.ToEnvelope(),
                    personAssignedToDelegationRemoved.ToEnvelope(),
                }
            );

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single()
                .Delegations
                .Count.Should().Be(0);
        }

        [EnvVarIgnoreFact]
        public async void PersonRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var personUpdated = scenario.Create<PersonUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedPersonToBodySeat.ToEnvelope(),
                    personUpdated.ToEnvelope(),
                }
            );

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            delegation.BodyMandateId.Should().Be(assignedPersonToBodySeat.BodyMandateId);
            delegation.PersonId.Should().Be(assignedPersonToBodySeat.PersonId);
            delegation.PersonName.Should().Be(personUpdated.FirstName + " " + personUpdated.Name);
        }

        [EnvVarIgnoreFact]
        public async void OrganisationRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var organisationInfoUpdated = scenario.Create<OrganisationInfoUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    organisationInfoUpdated.ToEnvelope(),
                }
            );

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            delegation.BodyMandateId.Should().Be(assignedOrganisationToBodySeat.BodyMandateId);
            delegation.OrganisationId.Should().Be(assignedOrganisationToBodySeat.OrganisationId);
            delegation.OrganisationName.Should().Be(organisationInfoUpdated.Name);
        }

        [EnvVarIgnoreFact]
        public async void FunctionTypeRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedFunctionTypeToBodySeat = scenario.Create<AssignedFunctionTypeToBodySeat>();
            var functionTypeUpdated = scenario.Create<FunctionUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedFunctionTypeToBodySeat.ToEnvelope(),
                    functionTypeUpdated.ToEnvelope(),
                }
            );

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            delegation.BodyMandateId.Should().Be(assignedFunctionTypeToBodySeat.BodyMandateId);
            delegation.FunctionTypeId.Should().Be(assignedFunctionTypeToBodySeat.FunctionTypeId);
            delegation.FunctionTypeName.Should().Be(functionTypeUpdated.Name);
        }

        [EnvVarIgnoreFact]
        public async void FunctionTypeOrOrganisationOrPersonRenamed_DoesNotCrashBecauseNoDocumentsFound()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var functionTypeUpdated = scenario.Create<FunctionUpdated>();
            var organisationInfoUpdated = scenario.Create<OrganisationInfoUpdated>();
            var personUpdated = scenario.Create<PersonUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    functionTypeUpdated.ToEnvelope(),
                    organisationInfoUpdated.ToEnvelope(),
                    personUpdated.ToEnvelope(),
                }
            );
        }

        [EnvVarIgnoreFact]
        public async void SeatTypeUpdated_UpdatesBodySeatType()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase<BodyHandler>(
                new ParameterNameArg<Guid>("bodyId", bodyId));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var anotherBodySeatAdded = scenario.Create<BodySeatAdded>();
            scenario.AddCustomization(new ParameterNameArg<Guid>("seatTypeId", bodySeatAdded.SeatTypeId));
            var seatTypeUpdated = scenario.Create<SeatTypeUpdated>();

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    anotherBodySeatAdded.ToEnvelope(),
                    seatTypeUpdated.ToEnvelope(),
                }
            );

            var seats = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats;

            var bodySeat = seats.Single(seat => seat.BodySeatId == bodySeatAdded.BodySeatId);
            var anotherBodySeat = seats.Single(seat => seat.BodySeatId == anotherBodySeatAdded.BodySeatId);

            bodySeat.SeatTypeName.Should().Be(seatTypeUpdated.Name);
            anotherBodySeat.SeatTypeName.Should().Be(anotherBodySeat.SeatTypeName);
        }

        [EnvVarIgnoreFact]
        public async void PersonUpdated_UpdatesDelegationsPersonName()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenario(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personUpdated = scenario.Create<PersonUpdated>();
            var personAssignedToDelegation = scenario.CreatePersonAssignedToDelegation(
                bodyId,
                bodySeatAdded.BodySeatId,
                assignedOrganisationToBodySeat.BodyMandateId,
                $"{personUpdated.FirstName} {personUpdated.Name}");

            await _eventProcessor.Handle<BodyDocument>(
                new List<IEnvelope>
                {
                    initialiseProjection.ToEnvelope(),
                    bodyRegistered.ToEnvelope(),
                    bodySeatAdded.ToEnvelope(),
                    assignedOrganisationToBodySeat.ToEnvelope(),
                    personAssignedToDelegation.ToEnvelope(),
                    personUpdated.ToEnvelope(),
                }
            );

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single()
                .Delegations
                .Single();

            delegation.PersonName.Should().Be($"{personUpdated.FirstName} {personUpdated.Name}");
        }
    }
}
