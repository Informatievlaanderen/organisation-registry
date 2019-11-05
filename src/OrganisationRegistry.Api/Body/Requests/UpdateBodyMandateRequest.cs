namespace OrganisationRegistry.Api.Body.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.Function;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Person;
    using System.Collections.Generic;
    using System.Linq;
    using OrganisationRegistry.ContactType;

    public class UpdateBodyMandateInternalRequest
    {
        public Guid BodyId { get; }
        public UpdateBodyMandateRequest Body { get; }

        public UpdateBodyMandateInternalRequest(Guid bodyId, UpdateBodyMandateRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class UpdateBodyMandateRequest
    {
        public Guid BodyMandateId { get; set; }
        public Guid BodySeatId { get; set; }
        public BodyMandateType? BodyMandateType { get; set; }
        public Guid DelegatorId { get; set; }
        public Guid DelegatedId { get; set; }
        public Dictionary<Guid, string> Contacts { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateBodyMandateInternalRequestValidator : AbstractValidator<UpdateBodyMandateInternalRequest>
    {
        public UpdateBodyMandateInternalRequestValidator()
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

    public static class UpdateBodyMandateRequestMapping
    {
        public static ReassignPersonToBodySeat MapForPerson(UpdateBodyMandateInternalRequest message)
        {
            return new ReassignPersonToBodySeat(
                new BodyId(message.BodyId),
                new BodyMandateId(message.Body.BodyMandateId),
                new BodySeatId(message.Body.BodySeatId),
                new PersonId(message.Body.DelegatorId),
                message.Body.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }

        public static ReassignFunctionTypeToBodySeat MapForFunctionType(UpdateBodyMandateInternalRequest message)
        {
            return new ReassignFunctionTypeToBodySeat(
                new BodyId(message.BodyId),
                new BodyMandateId(message.Body.BodyMandateId),
                new BodySeatId(message.Body.BodySeatId),
                new OrganisationId(message.Body.DelegatorId),
                new FunctionTypeId(message.Body.DelegatedId),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }

        public static ReassignOrganisationToBodySeat MapForOrganisation(UpdateBodyMandateInternalRequest message)
        {
            return new ReassignOrganisationToBodySeat(
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
