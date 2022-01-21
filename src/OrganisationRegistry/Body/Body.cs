namespace OrganisationRegistry.Body
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BodyClassification;
    using BodyClassificationType;
    using ContactType;
    using Events;
    using Exceptions;
    using FormalFramework;
    using Function;
    using Infrastructure.Domain;
    using Infrastructure.Events;
    using LifecyclePhaseType;
    using Organisation;
    using Person;
    using SeatType;

    public class Body : AggregateRoot
    {
        private string _name;
        private string _bodyNumber;
        private string _shortName;
        private string _description;
        private bool _isLifecycleValid;

        private bool? _balancedParticipationObligatory;
        private string? _balancedParticipationExtraRemark;
        private string? _balancedParticipationExceptionMeasure;

        private Period _formalValidity;

        private readonly List<BodyFormalFramework> _bodyFormalFrameworks;
        private readonly List<BodyLifecyclePhase> _bodyLifecyclePhases;
        private readonly BodySeats _bodySeats;

        private readonly List<BodyOrganisation> _bodyOrganisations;
        private BodyOrganisation _currentBodyOrganisation;
        private readonly List<BodyContact> _bodyContacts;
        private readonly List<BodyBodyClassification> _bodyBodyClassifications;

        public Body()
        {
            _bodyOrganisations = new List<BodyOrganisation>();
            _bodyContacts = new List<BodyContact>();
            _bodyFormalFrameworks = new List<BodyFormalFramework>();
            _bodyLifecyclePhases = new List<BodyLifecyclePhase>();
            _bodySeats = new BodySeats();
            _bodyBodyClassifications = new List<BodyBodyClassification>();
        }

        public Body(
            BodyId id,
            string name,
            string bodyNumber,
            string shortName,
            Organisation organisation,
            string description,
            Period validity,
            Period formalValidity,
            LifecyclePhaseType activeLifecyclePhaseType,
            LifecyclePhaseType inactiveLifecyclePhaseType) : this()
        {
            ApplyChange(new BodyRegistered(
                id,
                name,
                bodyNumber,
                shortName,
                description,
                formalValidity.Start,
                formalValidity.End));

            if (IsNotTheDefaultActiveLifecyclePhaseType(activeLifecyclePhaseType))
                throw new IncorrectActiveLifecyclePhaseTypeDefinedInConfiguration();

            if (IsNotTheDefaultInactiveLifecyclePhaseType(inactiveLifecyclePhaseType))
                throw new IncorrectInactiveLifecyclePhaseTypeDefinedInConfiguration();

            if (validity.HasFixedEnd)
            {
                // There is an end, so we add 2 periods
                ApplyChange(new BodyLifecyclePhaseAdded(
                    Id,
                    Guid.NewGuid(),
                    activeLifecyclePhaseType.Id,
                    activeLifecyclePhaseType.Name,
                    activeLifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor,
                    validity.Start,
                    validity.End));

                ApplyChange(new BodyLifecyclePhaseAdded(
                    Id,
                    Guid.NewGuid(),
                    inactiveLifecyclePhaseType.Id,
                    inactiveLifecyclePhaseType.Name,
                    inactiveLifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor,
                    validity.End.DateTime.Value.AddDays(1),
                    null));
            }
            else
            {
                // There is only one period
                ApplyChange(new BodyLifecyclePhaseAdded(
                    Id,
                    Guid.NewGuid(),
                    activeLifecyclePhaseType.Id,
                    activeLifecyclePhaseType.Name,
                    activeLifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor,
                    validity.Start,
                    null));
            }

            if (validity.HasFixedStart)
                ApplyChange(new BodyLifecycleBecameValid(Id));

            if (organisation == null)
                return;

            ApplyChange(new BodyOrganisationAdded(
                Id,
                Id,
                _name,
                organisation.Id,
                organisation.State.Name,
                null,
                null));

            ApplyChange(new BodyAssignedToOrganisation(
                Id,
                _name,
                organisation.Id,
                organisation.State.Name,
                Id));
        }

        private static bool IsNotTheDefaultActiveLifecyclePhaseType(LifecyclePhaseType? activeLifecyclePhaseType)
        {
            return activeLifecyclePhaseType == null ||
                   activeLifecyclePhaseType.Status != LifecyclePhaseTypeStatus.Default ||
                   activeLifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor != LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;
        }

        private static bool IsNotTheDefaultInactiveLifecyclePhaseType(LifecyclePhaseType? inactiveLifecyclePhaseType)
        {
            return inactiveLifecyclePhaseType == null ||
                   inactiveLifecyclePhaseType.Status != LifecyclePhaseTypeStatus.Default ||
                   inactiveLifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor != LifecyclePhaseTypeIsRepresentativeFor.InactivePhase;
        }

        public void AssignBodyNumber(string bodyNumber)
        {
            if (!string.IsNullOrWhiteSpace(_bodyNumber))
                throw new BodyNumberAlreadyAssigned();

            ApplyChange(new BodyNumberAssigned(
                Id,
                bodyNumber));
        }

        public void AssignBodySeatNumberToBodySeat(
            BodySeatId bodySeatId,
            string bodySeatNumber)
        {
            var bodySeat = _bodySeats.Single(x => x.BodySeatId == bodySeatId);

            if (!string.IsNullOrWhiteSpace(bodySeat.BodySeatNumber))
                throw new BodySeatNumberAlreadyAssigned();

            ApplyChange(new BodySeatNumberAssigned(
                Id,
                bodySeatId,
                bodySeatNumber));
        }

        public void UpdateInfo(
            string name,
            string shortName,
            string description)
        {
            ApplyChange(new BodyInfoChanged(
                Id,
                name,
                shortName,
                description,
                _name,
                _shortName,
                _description));
        }

        public void UpdateFormalValidity(
            Period formalValidity,
            IDateTimeProvider dateTimeProvider)  // TODO: Can we remove 'dateTimeProvider' if it is not used?
        {
            ApplyChange(new BodyFormalValidityChanged(
                Id,
                formalValidity.Start,
                formalValidity.End,
                _formalValidity.Start,
                _formalValidity.End));
        }

        public void UpdateBalancedParticipation(
            bool? balancedParticipationObligatory,
            string? balancedParticipationExtraRemark,
            string? balancedParticipationExceptionMeasure)
        {
            ApplyChange(new BodyBalancedParticipationChanged(
                Id,
                balancedParticipationObligatory,
                balancedParticipationExtraRemark,
                balancedParticipationExceptionMeasure,
                _balancedParticipationObligatory,
                _balancedParticipationExtraRemark,
                _balancedParticipationExceptionMeasure));
        }

        public void AddFormalFramework(
            BodyFormalFrameworkId bodyFormalFrameworkId,
            FormalFramework formalFramework,
            Period validity)
        {
            if (_bodyFormalFrameworks
                .Where(bodyFormalFramework => bodyFormalFramework.FormalFrameworkId == formalFramework.Id)
                .Any(bodyFormalFramework => bodyFormalFramework.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToFormalFrameworkInThisPeriod();

            ApplyChange(new BodyFormalFrameworkAdded(
                Id,
                bodyFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                validity.Start,
                validity.End));
        }

        public void UpdateFormalFramework(
            BodyFormalFrameworkId bodyFormalFrameworkId,
            FormalFramework formalFramework,
            Period validity)
        {
            if (_bodyFormalFrameworks
                .Where(
                    bodyFormalFramework => bodyFormalFramework.BodyFormalFrameworkId != bodyFormalFrameworkId &&
                                           bodyFormalFramework.FormalFrameworkId == formalFramework.Id)
                .Any(bodyFormalFramework => bodyFormalFramework.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToFormalFrameworkInThisPeriod();

            var previousBodyFormalFramework = _bodyFormalFrameworks.Single(x => x.BodyFormalFrameworkId == bodyFormalFrameworkId);

            ApplyChange(new BodyFormalFrameworkUpdated(
                Id,
                bodyFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                validity.Start,
                validity.End,
                previousBodyFormalFramework.FormalFrameworkId,
                previousBodyFormalFramework.FormalFrameworkName,
                previousBodyFormalFramework.Validity.Start,
                previousBodyFormalFramework.Validity.End));
        }

        public void AddLifecyclePhase(
            BodyLifecyclePhaseId bodyLifecyclePhaseId,
            LifecyclePhaseType lifecyclePhaseType,
            Period validity)
        {
            if (_bodyLifecyclePhases.Any(bodyLifecyclePhase => bodyLifecyclePhase.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToLifecyclePhaseInThisPeriod();

            ApplyChange(new BodyLifecyclePhaseAdded(
                Id,
                bodyLifecyclePhaseId,
                lifecyclePhaseType.Id,
                lifecyclePhaseType.Name,
                lifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor,
                validity.Start,
                validity.End));

            // We don't need to do the above equivalent check for BodyBecameInactive when adding a LifecyclePhase, because that
            // would only be relevant if there wasn't already a lifecyclePhase defined for today,
            // and Body defaults to inactive if there's no period defined on today.

            CheckIfLifecycleValidityChanged();
        }

        public void UpdateLifecyclePhase(
            BodyLifecyclePhaseId bodyLifecyclePhaseId,
            LifecyclePhaseType lifecyclePhaseType,
            Period validity)
        {
            if (_bodyLifecyclePhases
                .Where(bodyLifecyclePhase => bodyLifecyclePhase.BodyLifecyclePhaseId != bodyLifecyclePhaseId)
                .Any(bodyLifecyclePhase => bodyLifecyclePhase.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToLifecyclePhaseInThisPeriod();

            var previousBodyLifecyclePhase = _bodyLifecyclePhases.Single(x => x.BodyLifecyclePhaseId == bodyLifecyclePhaseId);

            ApplyChange(new BodyLifecyclePhaseUpdated(
                Id,
                bodyLifecyclePhaseId,
                lifecyclePhaseType.Id,
                lifecyclePhaseType.Name,
                lifecyclePhaseType.LifecyclePhaseTypeIsRepresentativeFor,
                validity.Start,
                validity.End,
                previousBodyLifecyclePhase.LifecyclePhaseTypeId,
                previousBodyLifecyclePhase.LifecyclePhaseTypeName,
                previousBodyLifecyclePhase.LifecyclePhaseTypeIsRepresentativeFor,
                previousBodyLifecyclePhase.Validity.Start,
                previousBodyLifecyclePhase.Validity.End));

            CheckIfLifecycleValidityChanged();
        }

        public void AddSeat(
            BodySeatId bodySeatId,
            string name,
            string bodySeatNumber,
            SeatType seatType,
            bool paidSeat,
            bool entitledToVote,
            Period validity)
        {
            ApplyChange(new BodySeatAdded(
                Id,
                bodySeatId,
                name,
                bodySeatNumber,
                seatType.Id,
                seatType.Name,
                seatType.Order,
                seatType.IsEffective,
                paidSeat,
                entitledToVote,
                validity.Start,
                validity.End));
        }

        public void UpdateSeat(
            BodySeatId bodySeatId,
            string name,
            SeatType seatType,
            bool paidSeat,
            bool entitledToVote,
            Period validity)
        {
            var previousBodySeat = _bodySeats.Single(x => x.BodySeatId == bodySeatId);

            ApplyChange(new BodySeatUpdated(
                Id,
                bodySeatId,
                name,
                seatType.Id,
                seatType.Name,
                seatType.Order,
                seatType.IsEffective,
                paidSeat,
                entitledToVote,
                validity.Start,
                validity.End,
                previousBodySeat.Name,
                previousBodySeat.SeatType.Id,
                previousBodySeat.SeatType.Name,
                previousBodySeat.SeatType.Order,
                previousBodySeat.SeatType.IsEffective,
                previousBodySeat.PaidSeat,
                previousBodySeat.EntitledToVote,
                previousBodySeat.Validity.Start,
                previousBodySeat.Validity.End));
        }

        public void AddOrganisation(
            BodyOrganisationId bodyOrganisationId,
            Organisation organisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_bodyOrganisations.Any(bodyOrganisation => bodyOrganisation.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToOrganisationInThisPeriod();

            ApplyChange(new BodyOrganisationAdded(
                Id,
                bodyOrganisationId,
                _name,
                organisation.Id,
                organisation.State.Name,
                validity.Start,
                validity.End));

            CheckIfCurrentOrganisationChanged(
                new BodyOrganisation(
                    bodyOrganisationId,
                    new OrganisationId(organisation.Id),
                    organisation.State.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void UpdateOrganisation(
            BodyOrganisationId bodyOrganisationId,
            Organisation organisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_bodyOrganisations
                .Where(bodyOrganisation => bodyOrganisation.BodyOrganisationId != bodyOrganisationId)
                .Any(bodyOrganisation => bodyOrganisation.Validity.OverlapsWith(validity)))
                throw new BodyAlreadyCoupledToOrganisationInThisPeriod();

            var previousBodyOrganisation = _bodyOrganisations.Single(x => x.BodyOrganisationId == bodyOrganisationId);

            ApplyChange(new BodyOrganisationUpdated(
                Id,
                bodyOrganisationId,
                organisation.Id,
                organisation.State.Name,
                validity.Start,
                validity.End,
                previousBodyOrganisation.OrganisationId,
                previousBodyOrganisation.OrganisationName,
                previousBodyOrganisation.Validity.Start,
                previousBodyOrganisation.Validity.End));

            CheckIfCurrentOrganisationChanged(
                new BodyOrganisation(
                    bodyOrganisationId,
                    new OrganisationId(organisation.Id),
                    organisation.State.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void UpdateCurrentOrganisation(DateTime today)
        {
            var events = new List<IEvent>();

            if (_currentBodyOrganisation != null && !_currentBodyOrganisation.Validity.OverlapsWith(today))
                events.Add(
                    new BodyClearedFromOrganisation(
                        Id,
                        _currentBodyOrganisation.OrganisationId));

            var newCurrentBodyOrganisation = _bodyOrganisations.SingleOrDefault(organisation => organisation.Validity.OverlapsWith(today));

            if (newCurrentBodyOrganisation != null && !Equals(newCurrentBodyOrganisation, _currentBodyOrganisation))
                events.Add(
                    new BodyAssignedToOrganisation(
                        Id,
                        _name,
                        newCurrentBodyOrganisation.OrganisationId,
                        newCurrentBodyOrganisation.OrganisationName,
                        newCurrentBodyOrganisation.BodyOrganisationId));

            events.ForEach(ApplyChange);
        }

        public void AssignPersonToBodySeat(
            Person person,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            List<Contact> contacts,
            Period validity)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasABodyMandateWithOverlappingValidity(validity))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            if (_bodySeats.CanThisPersonBeAssignedInThisPeriod(new PersonId(person.Id), validity))
                throw new PersonAlreadyAssignedInThisPeriod();

            ApplyChange(new AssignedPersonToBodySeat(
                Id,
                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                person.Id,
                person.FirstName,
                person.Name,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End));
        }

        public void AssignFunctionTypeToBodySeat(
            Organisation organisation,
            FunctionType functionType,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            Period validity)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasABodyMandateWithOverlappingValidity(validity))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            ApplyChange(new AssignedFunctionTypeToBodySeat(
                Id,
                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                organisation.Id,
                organisation.State.Name,
                functionType.Id,
                functionType.Name,
                validity.Start,
                validity.End));
        }

        public void AssignOrganisationToBodySeat(
            Organisation organisation,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            Period validity)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasABodyMandateWithOverlappingValidity(validity))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            ApplyChange(new AssignedOrganisationToBodySeat(
                Id,
                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                organisation.Id,
                organisation.State.Name,
                validity.Start,
                validity.End));
        }

        public void ReassignPersonToBodySeat(
            Person person,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            List<Contact> contacts,
            Period validity)
        {
            var previousBodySeat = _bodySeats.Single(seat => seat.BodyMandates.Any(mandate => mandate.BodyMandateId == bodyMandateId));
            var previousBodyMandate = previousBodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);
            var previousPersonMandate = (PersonBodyMandate)previousBodyMandate;

            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasAnotherBodyMandateWithOverlappingValidity(validity, bodyMandateId))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            if (_bodySeats.CanThisPersonBeReassignedInThisPeriod(bodyMandateId, new PersonId(person.Id), validity))
                throw new PersonAlreadyAssignedInThisPeriod();

            ApplyChange(new ReassignedPersonToBodySeat(
                Id,

                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                person.Id,
                person.FirstName,
                person.Name,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End,

                previousBodySeat.BodySeatId,
                previousBodySeat.BodySeatNumber,
                previousBodySeat.Name,
                previousBodySeat.SeatType.Order,
                previousPersonMandate.PersonId,
                previousPersonMandate.PersonFirstName,
                previousPersonMandate.PersonName,
                previousPersonMandate.Contacts,
                previousBodyMandate.Validity.Start,
                previousBodyMandate.Validity.End));
        }

        public void ReassignFunctionTypeToBodySeat(
            Organisation organisation,
            FunctionType functionType,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            Period validity)
        {
            var previousBodySeat = _bodySeats.Single(seat => seat.BodyMandates.Any(mandate => mandate.BodyMandateId == bodyMandateId));
            var previousBodyMandate = previousBodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);
            var previousFunctionTypeMandate = (FunctionTypeBodyMandate)previousBodyMandate;

            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasAnotherBodyMandateWithOverlappingValidity(validity, bodyMandateId))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            ApplyChange(new ReassignedFunctionTypeToBodySeat(
                Id,

                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                organisation.Id,
                organisation.State.Name,
                functionType.Id,
                functionType.Name,
                validity.Start,
                validity.End,
                previousBodySeat.BodySeatId,
                previousBodySeat.BodySeatNumber,
                previousBodySeat.Name,
                previousBodySeat.SeatType.Order,
                previousFunctionTypeMandate.OrganisationId,
                previousFunctionTypeMandate.OrganisationName,
                previousFunctionTypeMandate.FunctionTypeId,
                previousFunctionTypeMandate.FunctionTypeName,
                previousBodyMandate.Validity.Start,
                previousBodyMandate.Validity.End));
        }

        public void ReassignOrganisationToBodySeat(
            Organisation organisation,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            Period validity)
        {
            var previousBodySeat = _bodySeats.Single(seat => seat.BodyMandates.Any(mandate => mandate.BodyMandateId == bodyMandateId));
            var previousBodyMandate = previousBodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);
            var previousOrganisationMandate = (OrganisationBodyMandate)previousBodyMandate;

            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);

            if (bodySeat.HasAnotherBodyMandateWithOverlappingValidity(validity, bodyMandateId))
                throw new BodyMandateAlreadyCoupledToBodySeatInThisPeriod();

            ApplyChange(new ReassignedOrganisationToBodySeat(
                Id,

                bodyMandateId,
                bodySeat.BodySeatId,
                bodySeat.BodySeatNumber,
                bodySeat.Name,
                bodySeat.SeatType.Order,
                organisation.Id,
                organisation.State.Name,
                validity.Start,
                validity.End,
                previousBodySeat.BodySeatId,
                previousBodySeat.BodySeatNumber,
                previousBodySeat.Name,
                previousBodySeat.SeatType.Order,
                previousOrganisationMandate.OrganisationId,
                previousOrganisationMandate.OrganisationName,
                previousBodyMandate.Validity.Start,
                previousBodyMandate.Validity.End));
        }

        public void AssignPersonToDelegation(
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId,
            DelegationAssignmentId delegationAssignmentId,
            Person person,
            List<Contact> contacts,
            Period validity,
            DateTime today)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);

            if (bodyMandate.HasPersonDelegationWithOverlappingValidity(validity))
                throw new PersonDelegationAlreadyAssignedToBodyMandateInThisPeriod();

            ApplyChange(new PersonAssignedToDelegation(
                Id,
                bodySeatId,
                bodyMandateId,
                delegationAssignmentId,
                person.Id,
                person.FullName,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End));

            var newAssignment = bodyMandate.Assignments.Single(assignment => assignment.Id == delegationAssignmentId);
            CheckIfCurrentPersonAssignedToDelegationChanged(
                bodySeatId,
                bodyMandateId,
                bodyMandate.CurrentAssignment,
                newAssignment, today);
        }

        public void UpdatePersonAssignmentToDelegation(
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId,
            DelegationAssignmentId delegationAssignmentId,
            Person person,
            List<Contact> contacts,
            Period validity,
            DateTime today)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);

            if (bodyMandate.HasAnotherPersonDelegationWithOverlappingValidity(delegationAssignmentId, validity))
                throw new PersonDelegationAlreadyAssignedToBodyMandateInThisPeriod();

            var previousAssignment = bodyMandate.Assignments.Single(x => x.Id == delegationAssignmentId);

            ApplyChange(new PersonAssignedToDelegationUpdated(
                Id,
                bodySeatId,
                bodyMandateId,
                delegationAssignmentId,

                person.Id,
                person.FullName,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End,

                previousAssignment.PersonId,
                previousAssignment.PersonFullName,
                previousAssignment.Contacts,
                previousAssignment.Validity.Start,
                previousAssignment.Validity.End));

            var updatedAssignment = bodyMandate.Assignments.Single(assignment => assignment.Id == delegationAssignmentId);
            CheckIfCurrentPersonAssignedToDelegationChanged(
                bodySeatId,
                bodyMandateId,
                bodyMandate.CurrentAssignment,
                updatedAssignment,
                today);
        }

        public void UpdateCurrentPersonAssignedToBodyMandate(
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId,
            DateTime today)
        {
            var events = new List<IEvent>();

            var bodyMandate =
                _bodySeats
                    .Single(seat => seat.BodySeatId == bodySeatId)
                    .BodyMandates
                    .Single(mandate => mandate.BodyMandateId == bodyMandateId);

            var currentAssignment = bodyMandate.CurrentAssignment;
            if (currentAssignment != null && !currentAssignment.Validity.OverlapsWith(today))
                events.Add(
                    new AssignedPersonClearedFromBodyMandate(
                        Id,
                        bodySeatId,
                        bodyMandateId,
                        currentAssignment.Id));

            var newCurrentAssignment = bodyMandate.Assignments.SingleOrDefault(assignments => assignments.Validity.OverlapsWith(today));

            if (newCurrentAssignment != null && !Equals(newCurrentAssignment, currentAssignment))
                events.Add(
                    new AssignedPersonAssignedToBodyMandate(
                        Id,
                        _name,
                        bodySeatId,
                        bodyMandateId,
                        newCurrentAssignment.Id,
                        newCurrentAssignment.PersonId,
                        newCurrentAssignment.PersonFullName,
                        newCurrentAssignment.Validity.Start,
                        newCurrentAssignment.Validity.End));

            events.ForEach(ApplyChange);
        }

        public void RemovePersonAssignmentFromDelegation(
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId,
            DelegationAssignmentId delegationAssignmentId,
            DateTime today) // TODO: Can we remove 'today' if it is not used?
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == bodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId);

            var assignmentToDelete = bodyMandate.Assignments.Single(x => x.Id == delegationAssignmentId);

            ApplyChange(new PersonAssignedToDelegationRemoved(
                Id,
                bodySeatId,
                bodyMandateId,
                delegationAssignmentId,

                assignmentToDelete.PersonId,
                assignmentToDelete.PersonFullName,
                assignmentToDelete.Contacts,
                assignmentToDelete.Validity.Start,
                assignmentToDelete.Validity.End));

            if (bodyMandate.CurrentAssignment?.Id == delegationAssignmentId)
                ApplyChange(new AssignedPersonClearedFromBodyMandate(
                    Id,
                    bodySeatId,
                    bodyMandateId,
                    delegationAssignmentId));
        }

        public void AddContact(
            Guid bodyContactId,
            ContactType contactType,
            string contactValue,
            Period validity)
        {
            ApplyChange(new BodyContactAdded(
                Id,
                bodyContactId,
                contactType.Id,
                contactType.Name,
                contactValue,
                validity.Start,
                validity.End));
        }

        public void UpdateContact(
            Guid bodyContactId,
            ContactType contactType,
            string contactValue,
            Period validity)
        {
            var previousContact =
                _bodyContacts.Single(contact => contact.BodyContactId == bodyContactId);

            ApplyChange(new BodyContactUpdated(
                Id,
                bodyContactId,
                contactType.Id,
                contactType.Name,
                contactValue,
                validity.Start,
                validity.End,
                previousContact.ContactTypeId,
                previousContact.ContactTypeName,
                previousContact.Value,
                previousContact.Validity.Start,
                previousContact.Validity.End));
        }

        public void AddBodyClassification(
            Guid bodyBodyClassificationId,
            BodyClassificationType bodyClassificationType,
            BodyClassification bodyClassification,
            Period validity)
        {
            if (_bodyBodyClassifications
                .Where(bodyBodyClassification => bodyBodyClassification.BodyClassificationTypeId == bodyClassificationType.Id)
                .Where(bodyBodyClassification => bodyBodyClassification.BodyBodyClassificationId != bodyBodyClassificationId)
                .Any(bodyBodyClassification => bodyBodyClassification.Validity.OverlapsWith(validity)))
                throw new BodyClassificationTypeAlreadyCoupledToInThisPeriod();

            ApplyChange(new BodyBodyClassificationAdded(
                Id,
                bodyBodyClassificationId,
                bodyClassificationType.Id,
                bodyClassificationType.Name,
                bodyClassification.Id,
                bodyClassification.Name,
                validity.Start,
                validity.End));
        }

        public void UpdateBodyClassification(
            Guid bodyBodyClassificationId,
            BodyClassificationType bodyClassificationType,
            BodyClassification bodyClassification,
            Period validity)
        {
            if (bodyClassification == null)
                throw new ArgumentNullException(nameof(bodyClassification));

            if (_bodyBodyClassifications
                .Where(bodyBodyClassification => bodyBodyClassification.BodyClassificationTypeId == bodyClassificationType.Id)
                .Where(bodyBodyClassification => bodyBodyClassification.BodyBodyClassificationId != bodyBodyClassificationId)
                .Any(bodyBodyClassification => bodyBodyClassification.Validity.OverlapsWith(validity)))
                throw new BodyClassificationTypeAlreadyCoupledToInThisPeriod();

            var previousBodyBodyClassification =
                _bodyBodyClassifications.Single(classification =>
                    classification.BodyBodyClassificationId == bodyBodyClassificationId);

            ApplyChange(new BodyBodyClassificationUpdated(
                Id,
                bodyBodyClassificationId,
                bodyClassificationType.Id, bodyClassificationType.Name,
                bodyClassification.Id, bodyClassification.Name,
                validity.Start, validity.End,
                previousBodyBodyClassification.BodyClassificationTypeId,
                previousBodyBodyClassification.BodyClassificationTypeName,
                previousBodyBodyClassification.BodyClassificationId,
                previousBodyBodyClassification.BodyClassificationName,
                previousBodyBodyClassification.Validity.Start,
                previousBodyBodyClassification.Validity.End));
        }

        private void CheckIfCurrentOrganisationChanged(
            BodyOrganisation bodyOrganisation,
            DateTime today)
        {
            if (_currentBodyOrganisation == null && bodyOrganisation.Validity.OverlapsWith(today))
            {
                ApplyChange(new BodyAssignedToOrganisation(
                    Id,
                    _name,
                    bodyOrganisation.OrganisationId,
                    bodyOrganisation.OrganisationName,
                    bodyOrganisation.BodyOrganisationId));
            }
            else if (!Equals(_currentBodyOrganisation, bodyOrganisation) && bodyOrganisation.Validity.OverlapsWith(today))
            {
                if (_currentBodyOrganisation != null)
                    ApplyChange(new BodyClearedFromOrganisation(
                        Id,
                        _currentBodyOrganisation.OrganisationId));

                ApplyChange(new BodyAssignedToOrganisation(
                    Id,
                    _name,
                    bodyOrganisation.OrganisationId,
                    bodyOrganisation.OrganisationName,
                    bodyOrganisation.BodyOrganisationId));
            }
            else if (Equals(_currentBodyOrganisation, bodyOrganisation) && !bodyOrganisation.Validity.OverlapsWith(today))
            {
                ApplyChange(new BodyClearedFromOrganisation(
                    Id,
                    bodyOrganisation.OrganisationId));
            }
        }

        private void CheckIfCurrentPersonAssignedToDelegationChanged(
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId,
            Assignment currentAssignment,
            Assignment assignment,
            DateTime today)
        {
            if (currentAssignment == null && assignment.Validity.OverlapsWith(today))
            {
                ApplyChange(new AssignedPersonAssignedToBodyMandate(
                    Id,
                    _name,
                    bodySeatId,
                    bodyMandateId,
                    assignment.Id,
                    assignment.PersonId,
                    assignment.PersonFullName,
                    assignment.Validity.Start,
                    assignment.Validity.End));
            }
            else if (!Equals(currentAssignment?.Id, assignment.Id) && assignment.Validity.OverlapsWith(today))
            {
                if (currentAssignment != null)
                    ApplyChange(new AssignedPersonClearedFromBodyMandate(
                        Id,
                        bodySeatId,
                        bodyMandateId,
                        currentAssignment.Id));

                ApplyChange(new AssignedPersonAssignedToBodyMandate(
                    Id,
                    _name,
                    bodySeatId,
                    bodyMandateId,
                    assignment.Id,
                    assignment.PersonId,
                    assignment.PersonFullName,
                    assignment.Validity.Start,
                    assignment.Validity.End));
            }
            else if (Equals(currentAssignment?.Id, assignment.Id) && !assignment.Validity.OverlapsWith(today))
            {
                if (currentAssignment != null)
                    ApplyChange(new AssignedPersonClearedFromBodyMandate(
                        Id,
                        bodySeatId,
                        bodyMandateId,
                        currentAssignment.Id));
            }
        }

        private void CheckIfLifecycleValidityChanged()
        {
            var isLifecycleValid = !CheckIfLifecycleHasGaps();

            if (_isLifecycleValid == isLifecycleValid)
                return;

            if (_isLifecycleValid && !isLifecycleValid)
                ApplyChange(new BodyLifecycleBecameInvalid(Id));

            if (!_isLifecycleValid && isLifecycleValid)
                ApplyChange(new BodyLifecycleBecameValid(Id));
        }

        private bool CheckIfLifecycleHasGaps()
        {
            // If there is no lifecycle, it has no gaps
            if (_bodyLifecyclePhases.Count == 0)
                return false;

            var sortedLifecyclePhases = _bodyLifecyclePhases.OrderBy(x => x.Validity.Start).ToList();

            // Should start with a fixed startdate
            if (!sortedLifecyclePhases.First().Validity.HasFixedStart)
                return true;

            // Should end with an infinite enddate
            if (sortedLifecyclePhases.Last().Validity.HasFixedEnd)
                return true;

            // All periouds should be continuous
            return sortedLifecyclePhases
                .Skip(1)
                .Zip(sortedLifecyclePhases, (current, previous) => new { EndDate = previous.Validity.End, StartDate = current.Validity.Start })
                .Any(x => x.EndDate.DateTime.Value.AddDays(1) != x.StartDate.DateTime);
        }

        private void Apply(BodyRegistered @event)
        {
            Id = @event.BodyId;
            _name = @event.Name;
            _bodyNumber = @event.BodyNumber;
            _shortName = @event.ShortName;
            _description = @event.Description;

            _formalValidity = new Period(
                new ValidFrom(@event.FormalValidFrom),
                new ValidTo(@event.FormalValidTo));
        }

        private void Apply(BodyNumberAssigned @event)
        {
            _bodyNumber = @event.BodyNumber;
        }

        private void Apply(BodySeatNumberAssigned @event)
        {
            var bodySeat = _bodySeats.Single(x => x.BodySeatId == @event.BodySeatId);

            bodySeat.AssignBodySeatNumber(@event.BodySeatNumber);
        }

        private void Apply(BodyLifecycleBecameValid @event)
        {
            _isLifecycleValid = true;
        }

        private void Apply(BodyLifecycleBecameInvalid @event)
        {
            _isLifecycleValid = false;
        }

        private void Apply(BodyAssignedToOrganisation @event)
        {
            _currentBodyOrganisation = _bodyOrganisations.Single(ob => ob.BodyOrganisationId == @event.BodyOrganisationId);
        }

        private void Apply(BodyClearedFromOrganisation @event)
        {
            _currentBodyOrganisation = null;
        }

        private void Apply(BodyInfoChanged @event)
        {
            _name = @event.Name;
            _shortName = @event.ShortName;
            _description = @event.Description;
        }

        private void Apply(BodyFormalValidityChanged @event)
        {
            _formalValidity = new Period(
                new ValidFrom(@event.FormalValidFrom),
                new ValidTo(@event.FormalValidTo));
        }

        private void Apply(BodyBalancedParticipationChanged @event)
        {
            _balancedParticipationObligatory = @event.BalancedParticipationObligatory;
            _balancedParticipationExtraRemark = @event.BalancedParticipationExtraRemark;
            _balancedParticipationExceptionMeasure = @event.BalancedParticipationExceptionMeasure;
        }

        private void Apply(BodyFormalFrameworkAdded @event)
        {
            _bodyFormalFrameworks.Add(
                new BodyFormalFramework(
                    new BodyFormalFrameworkId(@event.BodyFormalFrameworkId),
                    new FormalFrameworkId(@event.FormalFrameworkId),
                    @event.FormalFrameworkName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyFormalFrameworkUpdated @event)
        {
            var newBodyFormalFramework =
                new BodyFormalFramework(
                    new BodyFormalFrameworkId(@event.BodyFormalFrameworkId),
                    new FormalFrameworkId(@event.FormalFrameworkId),
                    @event.FormalFrameworkName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo)));

            _bodyFormalFrameworks.Remove(newBodyFormalFramework);
            _bodyFormalFrameworks.Add(newBodyFormalFramework);
        }

        private void Apply(BodyOrganisationAdded @event)
        {
            _bodyOrganisations.Add(
                new BodyOrganisation(
                    new BodyOrganisationId(@event.BodyOrganisationId),
                    new OrganisationId(@event.OrganisationId),
                    @event.OrganisationName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyOrganisationUpdated @event)
        {
            var newBodyOrganisation =
                new BodyOrganisation(
                    new BodyOrganisationId(@event.BodyOrganisationId),
                    new OrganisationId(@event.OrganisationId),
                    @event.OrganisationName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo)));

            _bodyOrganisations.Remove(newBodyOrganisation);
            _bodyOrganisations.Add(newBodyOrganisation);
        }

        private void Apply(BodyLifecyclePhaseAdded @event)
        {
            _bodyLifecyclePhases.Add(
                new BodyLifecyclePhase(
                    new BodyLifecyclePhaseId(@event.BodyLifecyclePhaseId),
                    new LifecyclePhaseTypeId(@event.LifecyclePhaseTypeId),
                    @event.LifecyclePhaseTypeName,
                    @event.LifecyclePhaseTypeIsRepresentativeFor,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyLifecyclePhaseUpdated @event)
        {
            var newBodyLifecyclePhase =
                new BodyLifecyclePhase(
                    new BodyLifecyclePhaseId(@event.BodyLifecyclePhaseId),
                    new LifecyclePhaseTypeId(@event.LifecyclePhaseTypeId),
                    @event.LifecyclePhaseTypeName,
                    @event.LifecyclePhaseTypeIsRepresentativeFor,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo)));

            _bodyLifecyclePhases.Remove(newBodyLifecyclePhase);
            _bodyLifecyclePhases.Add(newBodyLifecyclePhase);
        }

        private void Apply(BodySeatAdded @event)
        {
            _bodySeats.Add(
                new BodySeat(
                    new BodySeatId(@event.BodySeatId),
                    @event.Name,
                    @event.BodySeatNumber,
                    new SeatType(
                        new SeatTypeId(@event.SeatTypeId),
                        new SeatTypeName(@event.SeatTypeName),
                        @event.SeatTypeOrder,
                        @event.SeatTypeIsEffective ?? true),
                    @event.PaidSeat,
                    @event.EntitledToVote,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodySeatUpdated @event)
        {
            var oldBodySeat = _bodySeats.Single(x => x.BodySeatId == @event.BodySeatId);

            var newBodySeat =
                new BodySeat(
                    new BodySeatId(@event.BodySeatId),
                    @event.Name,
                    oldBodySeat.BodySeatNumber,
                    new SeatType(
                        new SeatTypeId(@event.SeatTypeId),
                        new SeatTypeName(@event.SeatTypeName),
                        @event.SeatTypeOrder,
                        @event.SeatTypeIsEffective ?? true),
                    @event.PaidSeat,
                    @event.EntitledToVote,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo)),
                    oldBodySeat.BodyMandates);

            _bodySeats.Remove(newBodySeat);
            _bodySeats.Add(newBodySeat);
        }

        private void Apply(AssignedPersonToBodySeat @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);

            bodySeat.AssignMandate(
                new PersonBodyMandate(
                    new BodyMandateId(@event.BodyMandateId),
                    new PersonId(@event.PersonId),
                    @event.PersonName,
                    @event.PersonFirstName,
                    @event.Contacts,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(AssignedFunctionTypeToBodySeat @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);

            bodySeat.AssignMandate(
                new FunctionTypeBodyMandate(
                    new BodyMandateId(@event.BodyMandateId),
                    new OrganisationId(@event.OrganisationId),
                    @event.OrganisationName,
                    new FunctionTypeId(@event.FunctionTypeId),
                    @event.FunctionTypeName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(AssignedOrganisationToBodySeat @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);

            bodySeat.AssignMandate(
                new OrganisationBodyMandate(
                    new BodyMandateId(@event.BodyMandateId),
                    new OrganisationId(@event.OrganisationId),
                    @event.OrganisationName,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(ReassignedPersonToBodySeat @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            var previousBodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.PreviousBodySeatId);

            previousBodySeat.UnassignMandate(new BodyMandateId(@event.BodyMandateId));

            bodySeat.AssignMandate(
                new PersonBodyMandate(
                    new BodyMandateId(@event.BodyMandateId),
                    new PersonId(@event.PersonId),
                    @event.PersonName,
                    @event.PersonFirstName,
                    @event.Contacts,
                    new Period(
                        new ValidFrom(@event.ValidFrom),
                        new ValidTo(@event.ValidTo))));
        }

        private void Apply(ReassignedFunctionTypeToBodySeat @event)
        {
            var previousBodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.PreviousBodySeatId);
            var previousBodyMandate = previousBodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == @event.BodyMandateId);

            previousBodySeat.UnassignMandate(previousBodyMandate);

            var newBodyMandate = new FunctionTypeBodyMandate(
                new BodyMandateId(@event.BodyMandateId),
                new OrganisationId(@event.OrganisationId),
                @event.OrganisationName,
                new FunctionTypeId(@event.FunctionTypeId),
                @event.FunctionTypeName,
                new Period(
                    new ValidFrom(@event.ValidFrom),
                    new ValidTo(@event.ValidTo)));

            newBodyMandate.SetAssignments(previousBodyMandate.Assignments);
            newBodyMandate.SetCurrentAssignment(previousBodyMandate.CurrentAssignment);

            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            bodySeat.AssignMandate(newBodyMandate);
        }

        private void Apply(ReassignedOrganisationToBodySeat @event)
        {
            var previousBodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.PreviousBodySeatId);
            var previousBodyMandate = previousBodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == @event.BodyMandateId);

            previousBodySeat.UnassignMandate(previousBodyMandate);

            var newBodyMandate = new OrganisationBodyMandate(
                new BodyMandateId(@event.BodyMandateId),
                new OrganisationId(@event.OrganisationId),
                @event.OrganisationName,
                new Period(
                    new ValidFrom(@event.ValidFrom),
                    new ValidTo(@event.ValidTo)));

            newBodyMandate.SetAssignments(previousBodyMandate.Assignments);
            newBodyMandate.SetCurrentAssignment(previousBodyMandate.CurrentAssignment);

            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            bodySeat.AssignMandate(newBodyMandate);
        }

        private void Apply(PersonAssignedToDelegation @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == @event.BodyMandateId);

            bodyMandate.AssignPerson(
                new DelegationAssignmentId(@event.DelegationAssignmentId),
                new PersonId(@event.PersonId),
                @event.PersonFullName,
                @event.Contacts,
                new Period(
                    new ValidFrom(@event.ValidFrom),
                    new ValidTo(@event.ValidTo)));
        }

        private void Apply(PersonAssignedToDelegationUpdated @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == @event.BodyMandateId);

            bodyMandate.RemoveAssignment(new DelegationAssignmentId(@event.DelegationAssignmentId));
            bodyMandate.AssignPerson(
                new DelegationAssignmentId(@event.DelegationAssignmentId),
                new PersonId(@event.PersonId),
                @event.PersonFullName,
                @event.Contacts,
                new Period(
                    new ValidFrom(@event.ValidFrom),
                    new ValidTo(@event.ValidTo)));
        }

        private void Apply(PersonAssignedToDelegationRemoved @event)
        {
            var bodySeat = _bodySeats.Single(seat => seat.BodySeatId == @event.BodySeatId);
            var bodyMandate = bodySeat.BodyMandates.Single(mandate => mandate.BodyMandateId == @event.BodyMandateId);

            bodyMandate.RemoveAssignment(new DelegationAssignmentId(@event.DelegationAssignmentId));
        }

        private void Apply(AssignedPersonAssignedToBodyMandate @event)
        {
            _bodySeats
                .Single(seat => seat.BodySeatId == @event.BodySeatId)
                .BodyMandates
                .Single(mandate => mandate.BodyMandateId == @event.BodyMandateId)
                .SetCurrentAssignment(@event.DelegationAssignmentId);
        }

        private void Apply(AssignedPersonClearedFromBodyMandate @event)
        {
            _bodySeats
                .Single(seat => seat.BodySeatId == @event.BodySeatId)
                .BodyMandates
                .Single(mandate => mandate.BodyMandateId == @event.BodyMandateId)
                .ClearCurrentAssignment();
        }

        private void Apply(BodyContactAdded @event)
        {
            _bodyContacts.Add(new BodyContact(
                @event.BodyContactId,
                @event.BodyId,
                @event.ContactTypeId,
                @event.ContactTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyContactUpdated @event)
        {
            _bodyContacts.Remove(_bodyContacts.Single(ob => ob.BodyContactId == @event.BodyContactId));
            _bodyContacts.Add(
                new BodyContact(
                    @event.BodyContactId,
                    @event.BodyId,
                    @event.ContactTypeId,
                    @event.ContactTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyBodyClassificationAdded @event)
        {
            _bodyBodyClassifications.Add(new BodyBodyClassification(
                @event.BodyBodyClassificationId,
                @event.BodyId,
                @event.BodyClassificationTypeId,
                @event.BodyClassificationTypeName,
                @event.BodyClassificationId,
                @event.BodyClassificationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(BodyBodyClassificationUpdated @event)
        {
            _bodyBodyClassifications.Remove(_bodyBodyClassifications.Single(ob => ob.BodyBodyClassificationId == @event.BodyBodyClassificationId));
            _bodyBodyClassifications.Add(
                new BodyBodyClassification(
                    @event.BodyBodyClassificationId,
                    @event.BodyId,
                    @event.BodyClassificationTypeId,
                    @event.BodyClassificationTypeName,
                    @event.BodyClassificationId,
                    @event.BodyClassificationName,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }
    }
}
