namespace OrganisationRegistry.Body
{
    using System.Linq;
    using BodyClassification;
    using BodyClassificationType;
    using Commands;
    using ContactType;
    using FormalFramework;
    using Function;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using LifecyclePhaseType;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using Person;
    using SeatType;

    public class BodyCommandHandlers :
        BaseCommandHandler<BodyCommandHandlers>,
        ICommandHandler<RegisterBody>,
        ICommandHandler<UpdateBodyInfo>,
        ICommandHandler<UpdateBodyValidity>,
        ICommandHandler<UpdateBodyBalancedParticipation>,
        ICommandHandler<AddBodyFormalFramework>,
        ICommandHandler<UpdateBodyFormalFramework>,
        ICommandHandler<AddBodyOrganisation>,
        ICommandHandler<UpdateBodyOrganisation>,
        ICommandHandler<UpdateCurrentBodyOrganisation>,
        ICommandHandler<AddBodyLifecyclePhase>,
        ICommandHandler<UpdateBodyLifecyclePhase>,
        ICommandHandler<AddBodySeat>,
        ICommandHandler<UpdateBodySeat>,
        ICommandHandler<AssignBodyNumber>,
        ICommandHandler<AssignBodySeatNumber>,
        ICommandHandler<AssignPersonToBodySeat>,
        ICommandHandler<AssignFunctionTypeToBodySeat>,
        ICommandHandler<AssignOrganisationToBodySeat>,
        ICommandHandler<ReassignPersonToBodySeat>,
        ICommandHandler<ReassignFunctionTypeToBodySeat>,
        ICommandHandler<ReassignOrganisationToBodySeat>,
        ICommandHandler<AddDelegationAssignment>,
        ICommandHandler<UpdateDelegationAssignment>,
        ICommandHandler<UpdateCurrentPersonAssignedToBodyMandate>,
        ICommandHandler<RemoveDelegationAssignment>,
        ICommandHandler<AddBodyContact>,
        ICommandHandler<UpdateBodyContact>,
        ICommandHandler<AddBodyBodyClassification>,
        ICommandHandler<UpdateBodyBodyClassification>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBodyNumberGenerator _bodyNumberGenerator;
        private readonly IUniqueBodyNumberValidator _uniqueBodyNumberValidator;
        private readonly IBodySeatNumberGenerator _bodySeatNumberGenerator;

        public BodyCommandHandlers(
            ILogger<BodyCommandHandlers> logger,
            ISession session,
            IDateTimeProvider dateTimeProvider,
            IBodyNumberGenerator bodyNumberGenerator,
            IUniqueBodyNumberValidator uniqueBodyNumberValidator,
            IBodySeatNumberGenerator bodySeatNumberGenerator) : base(logger, session)
        {
            _dateTimeProvider = dateTimeProvider;
            _bodyNumberGenerator = bodyNumberGenerator;
            _uniqueBodyNumberValidator = uniqueBodyNumberValidator;
            _bodySeatNumberGenerator = bodySeatNumberGenerator;
        }

        public void Handle(RegisterBody message)
        {
            if (_uniqueBodyNumberValidator.IsBodyNumberTaken(message.BodyNumber))
                throw new BodyNumberNotUniqueException();

            var bodyNumber = string.IsNullOrWhiteSpace(message.BodyNumber)
                ? _bodyNumberGenerator.GenerateNumber()
                : message.BodyNumber;

            var organisation =
                message.OrganisationId != null
                    ? Session.Get<Organisation>(message.OrganisationId)
                    : null;

            var activeLifecyclePhaseType =
                message.ActiveLifecyclePhaseTypeId != null
                    ? Session.Get<LifecyclePhaseType>(message.ActiveLifecyclePhaseTypeId)
                    : null;

            var inActiveLifecyclePhaseType =
                message.InactiveLifecyclePhaseTypeId != null
                    ? Session.Get<LifecyclePhaseType>(message.InactiveLifecyclePhaseTypeId)
                    : null;

            var body = new Body(
                message.BodyId,
                message.Name,
                bodyNumber,
                message.ShortName,
                organisation,
                message.Description,
                message.Validity,
                message.FormalValidity,
                activeLifecyclePhaseType,
                inActiveLifecyclePhaseType);

            Session.Add(body);
            Session.Commit();
        }

        public void Handle(UpdateBodyInfo message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateInfo(
                message.Name,
                message.ShortName,
                message.Description);

            Session.Commit();
        }

        public void Handle(UpdateBodyValidity message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateFormalValidity(
                message.FormalValidity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateBodyBalancedParticipation message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateBalancedParticipation(
                message.BalancedParticipationObligatory,
                message.BalancedParticipationExtraRemark,
                message.BalancedParticipationExceptionMeasure);

            Session.Commit();
        }

        public void Handle(AddBodyFormalFramework message)
        {
            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var body = Session.Get<Body>(message.BodyId);

            body.AddFormalFramework(
                message.BodyFormalFrameworkId,
                formalFramework,
                message.Validity);

            Session.Commit();
        }

        public void Handle(UpdateBodyFormalFramework message)
        {
            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateFormalFramework(
                message.BodyFormalFrameworkId,
                formalFramework,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AddBodyOrganisation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            var body = Session.Get<Body>(message.BodyId);

            body.AddOrganisation(
                message.BodyOrganisationId,
                organisation,
                message.Validity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateBodyOrganisation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateOrganisation(
                message.BodyOrganisationId,
                organisation,
                message.Validity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateCurrentBodyOrganisation message)
        {
            var body = Session.Get<Body>(message.BodyId);
            body.UpdateCurrentOrganisation(_dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(AddBodyLifecyclePhase message)
        {
            var lifecyclePhaseType = Session.Get<LifecyclePhaseType>(message.LifecyclePhaseTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.AddLifecyclePhase(
                message.BodyLifecyclePhaseId,
                lifecyclePhaseType,
                message.Validity);

            Session.Commit();
        }

        public void Handle(UpdateBodyLifecyclePhase message)
        {
            var lifecyclePhaseType = Session.Get<LifecyclePhaseType>(message.LifecyclePhaseTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateLifecyclePhase(
                message.BodyLifecyclePhaseId,
                lifecyclePhaseType,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AddBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var seatType = Session.Get<SeatType>(message.SeatTypeId);
            var bodySeatNumber = _bodySeatNumberGenerator.GenerateNumber();

            body.AddSeat(
                message.BodySeatId,
                message.Name,
                bodySeatNumber,
                seatType,
                message.PaidSeat,
                message.EntitledToVote,
                message.Validity);

            Session.Commit();
        }

        public void Handle(UpdateBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var seatType = Session.Get<SeatType>(message.SeatTypeId);

            body.UpdateSeat(
                message.BodySeatId,
                message.Name,
                seatType,
                message.PaidSeat,
                message.EntitledToVote,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AssignBodyNumber message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.AssignBodyNumber(_bodyNumberGenerator.GenerateNumber());

            Session.Commit();
        }

        public void Handle(AssignBodySeatNumber message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var bodySeatNumber = _bodySeatNumberGenerator.GenerateNumber();

            body.AssignBodySeatNumberToBodySeat(
                message.BodySeatId,
                bodySeatNumber);

            Session.Commit();
        }

        public void Handle(AssignPersonToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var person = Session.Get<Person>(message.PersonId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            body.AssignPersonToBodySeat(
                person,
                message.BodyMandateId,
                message.BodySeatId,
                contacts,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AssignFunctionTypeToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            var functionType = Session.Get<FunctionType>(message.FunctionTypeId);

            body.AssignFunctionTypeToBodySeat(
                organisation,
                functionType,
                message.BodyMandateId,
                message.BodySeatId,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AssignOrganisationToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            body.AssignOrganisationToBodySeat(
                organisation,
                message.BodyMandateId,
                message.BodySeatId,
                message.Validity);

            Session.Commit();
        }

        public void Handle(ReassignPersonToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var person = Session.Get<Person>(message.PersonId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            body.ReassignPersonToBodySeat(
                person,
                message.BodyMandateId,
                message.BodySeatId,
                contacts,
                message.Validity);

            Session.Commit();
        }

        public void Handle(ReassignFunctionTypeToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            var functionType = Session.Get<FunctionType>(message.FunctionTypeId);

            body.ReassignFunctionTypeToBodySeat(
                organisation,
                functionType,
                message.BodyMandateId,
                message.BodySeatId,
                message.Validity);

            Session.Commit();
        }

        public void Handle(ReassignOrganisationToBodySeat message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            body.ReassignOrganisationToBodySeat(
                organisation,
                message.BodyMandateId,
                message.BodySeatId,
                message.Validity);

            Session.Commit();
        }

        public void Handle(AddDelegationAssignment message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var person = Session.Get<Person>(message.PersonId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            body.AssignPersonToDelegation(
                message.BodySeatId,
                message.BodyMandateId,
                message.DelegationAssignmentId,
                person,
                contacts,
                message.Validity,
                _dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(UpdateDelegationAssignment message)
        {
            var body = Session.Get<Body>(message.BodyId);
            var person = Session.Get<Person>(message.PersonId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            body.UpdatePersonAssignmentToDelegation(
                message.BodySeatId,
                message.BodyMandateId,
                message.DelegationAssignmentId,
                person,
                contacts,
                message.Validity,
                _dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(UpdateCurrentPersonAssignedToBodyMandate message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateCurrentPersonAssignedToBodyMandate(
                message.BodySeatId,
                message.BodyMandateId,
                _dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(RemoveDelegationAssignment message)
        {
            var body = Session.Get<Body>(message.BodyId);

            body.RemovePersonAssignmentFromDelegation(
                message.BodySeatId,
                message.BodyMandateId,
                message.DelegationAssignmentId,
                _dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(AddBodyContact message)
        {
            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.AddContact(
                message.BodyContactId,
                contactType,
                message.ContactValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateBodyContact message)
        {
            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateContact(
                message.BodyContactId,
                contactType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddBodyBodyClassification message)
        {
            var bodyClassification = Session.Get<BodyClassification>(message.BodyClassificationId);
            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.AddBodyClassification(
                message.BodyBodyClassificationId,
                bodyClassificationType,
                bodyClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateBodyBodyClassification message)
        {
            var bodyClassification = Session.Get<BodyClassification>(message.BodyClassificationId);
            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
            var body = Session.Get<Body>(message.BodyId);

            body.UpdateBodyClassification(
                message.BodyBodyClassificationId,
                bodyClassificationType,
                bodyClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }
    }
}
