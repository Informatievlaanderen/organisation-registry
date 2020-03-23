namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System.Linq;
    using Bodies;
    using Body.Events;
    using FluentAssertions;
    using Infrastructure.Events;
    using LifecyclePhaseType.Events;
    using Microsoft.Extensions.Logging;
    using Projections.Body;
    using Projections.Infrastructure;
    using Scenario;
    using Xunit;
    using System;
    using Moq;
    using Scenario.Specimen;
    using Function.Events;
    using Infrastructure.AppSpecific;
    using Organisation.Events;
    using Person.Events;
    using SeatType.Events;

    [Collection(nameof(ElasticSearchFixture))]
    public class BodyHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly BodyHandler _handler;

        public BodyHandlerTests(ElasticSearchFixture fixture)
        {
            var memoryCaches = new Mock<IMemoryCaches>();
            memoryCaches.Setup(caches => caches.ContactTypeNames[It.IsAny<Guid>()]).Returns("contact type");

            _fixture = fixture;
            _handler = new BodyHandler(
                logger: _fixture.LoggerFactory.CreateLogger<BodyHandler>(),
                elastic: _fixture.Elastic,
                contextFactory: _fixture.ContextFactory,
                elasticSearchOptions: _fixture.ElasticSearchOptions,
                memoryCaches: memoryCaches.Object);
        }

        [EnvVarIgnoreFact]
        public void InitializeProjection_CreatesIndex()
        {
            var scenario = new BodyScenarioBase(Guid.NewGuid());

            Handle(scenario.Create<InitialiseProjection>());

            var indices = _fixture.Elastic.ReadClient.GetIndex(_fixture.ElasticSearchOptions.Value.BodyWriteIndex).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public void BodyRegistered_CreatesDocument()
        {
            var scenario = new BodyScenarioBase(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();

            Handle(
                initialiseProjection,
                bodyRegistered);

            var bodyDocument = _fixture.Elastic.ReadClient.Get<BodyDocument>(bodyRegistered.BodyId);

            bodyDocument.Source.Name.Should().Be(bodyRegistered.Name);
            bodyDocument.Source.BodyNumber.Should().Be(bodyRegistered.BodyNumber);
            bodyDocument.Source.ShortName.Should().Be(bodyRegistered.ShortName);
            bodyDocument.Source.Description.Should().Be(bodyRegistered.Description);
            bodyDocument.Source.FormalValidity.Start.Should().BeNull();
            bodyDocument.Source.FormalValidity.End.Should().BeNull();
            bodyDocument.Source.LifecyclePhases.Count.Should().Be(0, "Lifecycle phases are added by a different event.");
        }

        [EnvVarIgnoreFact]
        public void LifecyclePhaseTypeUpdated_UpdatesExistingBodyLifecyclePhases()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodyLifecyclePhaseAdded = scenario.Create<BodyLifecyclePhaseAdded>();
            var lifecyclePhaseTypeUpdated = scenario.Create<LifecyclePhaseTypeUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodyLifecyclePhaseAdded,
                lifecyclePhaseTypeUpdated);

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .LifecyclePhases
                .Single()
                .LifecyclePhaseTypeName
                .Should().Be(lifecyclePhaseTypeUpdated.Name);
        }

        [EnvVarIgnoreFact]
        public void MultipleBodySeatAddeds_CreatesMultipleBodySeats()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase(
                new ParameterNameArg("bodyId", bodyId));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var anotherBodySeatAdded = scenario.Create<BodySeatAdded>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                anotherBodySeatAdded);

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Count.Should().Be(2);
        }

        [EnvVarIgnoreFact]
        public void AssignedPersonToBodySeat_AddsMandateToBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedPersonToBodySeat);

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(assignedPersonToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(null);
            bodyMandate.OrganisationName.Should().Be(null);
            bodyMandate.FunctionTypeId.Should().Be(null);
            bodyMandate.FunctionTypeName.Should().Be(null);
            bodyMandate.PersonId.Should().Be(assignedPersonToBodySeat.PersonId);
            bodyMandate.PersonName.Should().Be(assignedPersonToBodySeat.PersonFirstName + " " + assignedPersonToBodySeat.PersonName);
            bodyMandate.Validity.Start.Should().Be(null);
            bodyMandate.Validity.End.Should().Be(null);
        }

        [EnvVarIgnoreFact]
        public void ReassignedPersonToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var reassignedPersonToBodySeat = scenario.Create<ReassignedPersonToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedPersonToBodySeat,
                reassignedPersonToBodySeat);

            var bodyMandate = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single();

            bodyMandate.BodyMandateId.Should().Be(reassignedPersonToBodySeat.BodyMandateId);
            bodyMandate.OrganisationId.Should().Be(null);
            bodyMandate.OrganisationName.Should().Be(null);
            bodyMandate.FunctionTypeId.Should().Be(null);
            bodyMandate.FunctionTypeName.Should().Be(null);
            bodyMandate.PersonId.Should().Be(reassignedPersonToBodySeat.PersonId);
            bodyMandate.PersonName.Should().Be(reassignedPersonToBodySeat.PersonFirstName + " " + reassignedPersonToBodySeat.PersonName);
            bodyMandate.Validity.Start.Should().Be(null);
            bodyMandate.Validity.End.Should().Be(null);
        }

        [EnvVarIgnoreFact]
        public void ReassignedPersonToBodySeat_DoesNotUpdateOtherMandates()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase(
                new ParameterNameArg("bodyId", bodyId),
                new ParameterNameArg("lifecyclePhaseTypeId", Guid.NewGuid()),
                new ParameterNameArg("bodySeatId", Guid.NewGuid()));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var assignedOtherPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            scenario.AddCustomization(new ParameterNameArg("bodyMandateId", assignedPersonToBodySeat.BodyMandateId));
            var reassignedPersonToBodySeat = scenario.Create<ReassignedPersonToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedPersonToBodySeat,
                assignedOtherPersonToBodySeat,
                reassignedPersonToBodySeat);

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
            otherBodyMandate.OrganisationName.Should().Be(null);
            otherBodyMandate.FunctionTypeId.Should().Be(null);
            otherBodyMandate.FunctionTypeName.Should().Be(null);
            otherBodyMandate.PersonId.Should().Be(assignedOtherPersonToBodySeat.PersonId);
            otherBodyMandate.PersonName.Should().Be(assignedOtherPersonToBodySeat.PersonFirstName + " " + assignedOtherPersonToBodySeat.PersonName);
            otherBodyMandate.Validity.Start.Should().Be(null);
            otherBodyMandate.Validity.End.Should().Be(null);
        }

        [EnvVarIgnoreFact]
        public void MultipleAssignPersonToBodySeat_CreatesMultipleBodyMandates()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase(
                new ParameterNameArg("bodyId", bodyId),
                new ParameterNameArg("bodySeatId", Guid.NewGuid()));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var assignedOtherPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedPersonToBodySeat,
                assignedOtherPersonToBodySeat);

            _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Count.Should().Be(2);
        }

        [EnvVarIgnoreFact]
        public void ReassignedOrganisationToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var reassignedOrganisationToBodySeat = scenario.Create<ReassignedOrganisationToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                reassignedOrganisationToBodySeat);

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
            bodyMandate.FunctionTypeName.Should().Be(null);
            bodyMandate.PersonId.Should().Be(null);
            bodyMandate.PersonName.Should().Be(null);
            bodyMandate.Validity.Start.Should().Be(null);
            bodyMandate.Validity.End.Should().Be(null);
        }

        [EnvVarIgnoreFact]
        public void ReassignedFunctionTypeToBodySeat_UpdatesMandate()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedFunctionTypeToBodySeat = scenario.Create<AssignedFunctionTypeToBodySeat>();
            var reassignedFunctionToBodySeat = scenario.Create<ReassignedFunctionTypeToBodySeat>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedFunctionTypeToBodySeat,
                reassignedFunctionToBodySeat);

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
            bodyMandate.PersonName.Should().Be(null);
            bodyMandate.Validity.Start.Should().Be(null);
            bodyMandate.Validity.End.Should().Be(null);
        }

        [EnvVarIgnoreFact]
        public void PersonAssignedToDelegation_AddsDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.Create<PersonAssignedToDelegation>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                personAssignedToDelegation);

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single()
                .Delegations
                .Single();

            delegation.DelegationAssignmentId.Should().Be(personAssignedToDelegation.DelegationAssignmentId);
            delegation.PersonId.Should().Be(personAssignedToDelegation.PersonId);
            delegation.PersonName.Should().Be(personAssignedToDelegation.PersonFullName);
            delegation.Contacts.Should().HaveCount(personAssignedToDelegation.Contacts.Count);
            foreach (var contact in personAssignedToDelegation.Contacts)
            {
                var delegationContact = delegation.Contacts.Single(x => x.ContactTypeId == contact.Key);
                delegationContact.Value.Should().Be(contact.Value);
            }
        }

        [EnvVarIgnoreFact]
        public void PersonAssignedToDelegationUpdated_UpdatesDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.Create<PersonAssignedToDelegation>();
            var personAssignedToDelegationUpdated = scenario.Create<PersonAssignedToDelegationUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                personAssignedToDelegation,
                personAssignedToDelegationUpdated);

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single()
                .Delegations
                .Single();

            delegation.DelegationAssignmentId.Should().Be(personAssignedToDelegationUpdated.DelegationAssignmentId);
            delegation.PersonId.Should().Be(personAssignedToDelegationUpdated.PersonId);
            delegation.PersonName.Should().Be(personAssignedToDelegationUpdated.PersonFullName);
            delegation.Contacts.Should().HaveCount(personAssignedToDelegationUpdated.Contacts.Count);
            foreach (var contact in personAssignedToDelegationUpdated.Contacts)
            {
                var delegationContact = delegation.Contacts.Single(x => x.ContactTypeId == contact.Key);
                delegationContact.Value.Should().Be(contact.Value);
            }
        }

        [EnvVarIgnoreFact]
        public void PersonAssignedToDelegationRemoved_RemovesDelegation()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.Create<PersonAssignedToDelegation>();
            var personAssignedToDelegationUpdated = scenario.Create<PersonAssignedToDelegationUpdated>();
            var personAssignedToDelegationRemoved = scenario.Create<PersonAssignedToDelegationRemoved>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                personAssignedToDelegation,
                personAssignedToDelegationUpdated,
                personAssignedToDelegationRemoved);

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
        public void PersonRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedPersonToBodySeat = scenario.Create<AssignedPersonToBodySeat>();
            var personUpdated = scenario.Create<PersonUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedPersonToBodySeat,
                personUpdated);

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
        public void OrganisationRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var organisationInfoUpdated = scenario.Create<OrganisationInfoUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                organisationInfoUpdated);

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
        public void FunctionTypeRenamed_RenamesInBodySeat()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedFunctionTypeToBodySeat = scenario.Create<AssignedFunctionTypeToBodySeat>();
            var functionTypeUpdated = scenario.Create<FunctionUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedFunctionTypeToBodySeat,
                functionTypeUpdated);

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
        public void FunctionTypeOrOrganisationOrPersonRenamed_DoesNotCrashBecauseNoDocumentsFound()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var functionTypeUpdated = scenario.Create<FunctionUpdated>();
            var organisationInfoUpdated = scenario.Create<OrganisationInfoUpdated>();
            var personUpdated = scenario.Create<PersonUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                functionTypeUpdated,
                organisationInfoUpdated,
                personUpdated);
        }

        [EnvVarIgnoreFact]
        public void SeatTypeUpdated_UpdatesBodySeatType()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new ScenarioBase(
                new ParameterNameArg("bodyId", bodyId));

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var anotherBodySeatAdded = scenario.Create<BodySeatAdded>();
            scenario.AddCustomization(new ParameterNameArg("seatTypeId", bodySeatAdded.SeatTypeId));
            var seatTypeUpdated = scenario.Create<SeatTypeUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                anotherBodySeatAdded,
                seatTypeUpdated);

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
        public void PersonUpdated_UpdatesDelegationsPersonName()
        {
            var bodyId = Guid.NewGuid();
            var scenario = new BodyScenarioBase(bodyId);

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var bodyRegistered = scenario.Create<BodyRegistered>();
            var bodySeatAdded = scenario.Create<BodySeatAdded>();
            var assignedOrganisationToBodySeat = scenario.Create<AssignedOrganisationToBodySeat>();
            var personAssignedToDelegation = scenario.Create<PersonAssignedToDelegation>();
            var personUpdated = scenario.Create<PersonUpdated>();

            Handle(
                initialiseProjection,
                bodyRegistered,
                bodySeatAdded,
                assignedOrganisationToBodySeat,
                personAssignedToDelegation,
                personUpdated);

            var delegation = _fixture.Elastic.ReadClient
                .Get<BodyDocument>(bodyId)
                .Source
                .Seats
                .Single()
                .Mandates
                .Single()
                .Delegations
                .Single();

            delegation.PersonName.Should().Be(personUpdated.FirstName + " " + personUpdated.Name);
        }

        private void Handle(params IEvent[] envelopes)
        {
            foreach (var envelope in envelopes)
            {
                _handler.Handle(null, null, (dynamic)envelope.ToEnvelope());
            }
        }
    }
}
