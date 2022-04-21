namespace OrganisationRegistry.Api.Backoffice.Body.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using Function;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using ContactType;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Person;

    public class AddBodyMandateInternalRequest
    {
        public Guid BodyId { get; set; }
        public AddBodyMandateRequest Body { get; }

        public AddBodyMandateInternalRequest(Guid bodyId, AddBodyMandateRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class AddBodyMandateRequest
    {
        public Guid BodyMandateId { get; set; }
        public Guid BodySeatId { get; set; }
        public BodyMandateType? BodyMandateType { get; set; }
        public Guid DelegatorId { get; set; }
        public Guid? DelegatedId { get; set; }
        public Dictionary<Guid, string> Contacts { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddBodyMandateInternalRequestValidator : AbstractValidator<AddBodyMandateInternalRequest>
    {
        public AddBodyMandateInternalRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BodyMandateId)
                .NotEmpty()
                .WithMessage("Body Mandate Id is required.");

            RuleFor(x => x.Body.BodySeatId)
                .NotEmpty()
                .WithMessage("Body Seat Id is required.");

            RuleFor(x => x.Body.BodyMandateType)
                .NotNull()
                .NotEmpty()
                .WithMessage("Body Mandate Type is required.");

            RuleFor(x => x.Body.DelegatorId)
                .NotEmpty()
                .WithMessage("Delegator Id is required.");

            RuleFor(x => x.Body.DelegatedId)
                .NotEmpty()
                .WithMessage("Delegated Id is required.")
                .When(request => request.Body.BodyMandateType == BodyMandateType.FunctionType);

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class AddBodyMandateRequestMapping
    {
        public static AssignPersonToBodySeat MapForPerson(AddBodyMandateInternalRequest message)
        {
            return new AssignPersonToBodySeat(
                new BodyId(message.BodyId),
                new BodyMandateId(message.Body.BodyMandateId),
                new BodySeatId(message.Body.BodySeatId),
                new PersonId(message.Body.DelegatorId),
                message.Body.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }

        public static AssignFunctionTypeToBodySeat MapForFunctionType(AddBodyMandateInternalRequest message)
        {
            return new AssignFunctionTypeToBodySeat(
                new BodyId(message.BodyId),
                new BodyMandateId(message.Body.BodyMandateId),
                new BodySeatId(message.Body.BodySeatId),
                new OrganisationId(message.Body.DelegatorId),
                new FunctionTypeId(message.Body.DelegatedId.Value),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }

        public static AssignOrganisationToBodySeat MapForOrganisation(AddBodyMandateInternalRequest message)
        {
            return new AssignOrganisationToBodySeat(
                new BodyId(message.BodyId),
                new BodyMandateId(message.Body.BodyMandateId),
                new BodySeatId(message.Body.BodySeatId),
                new OrganisationId(message.Body.DelegatorId),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }
    }
}
