namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using OrganisationRegistry.Body;
using OrganisationRegistry.ContactType;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Person;

public class UpdateDelegationAssignmentInternalRequest
{
    public Guid BodyMandateId { get; set; }
    public UpdateDelegationAssignmentRequest Body { get; }

    public UpdateDelegationAssignmentInternalRequest(Guid bodyMandateId, UpdateDelegationAssignmentRequest message)
    {
        BodyMandateId = bodyMandateId;
        Body = message;
    }
}

public class UpdateDelegationAssignmentRequest
{
    public Guid DelegationAssignmentId { get; set; }
    public Guid BodyId { get; set; }
    public Guid BodySeatId { get; set; }
    public Guid PersonId { get; set; }
    public Dictionary<Guid, string>? Contacts { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateDelegationAssignmentInternalRequestValidator : AbstractValidator<UpdateDelegationAssignmentInternalRequest>
{
    public UpdateDelegationAssignmentInternalRequestValidator()
    {
        RuleFor(x => x.BodyMandateId)
            .NotEmpty()
            .WithMessage("Body Mandate Id is required.");

        RuleFor(x => x.Body.BodyId)
            .NotEmpty()
            .WithMessage("Body Id is required.");

        RuleFor(x => x.Body.BodySeatId)
            .NotEmpty()
            .WithMessage("Body Seat Id is required.");

        RuleFor(x => x.Body.DelegationAssignmentId)
            .NotEmpty()
            .WithMessage("Delegation Assignment Id is required.");

        RuleFor(x => x.Body.PersonId)
            .NotEmpty()
            .WithMessage("Person Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateDelegationAssignmentRequestMapping
{
    public static UpdateDelegationAssignment Map(UpdateDelegationAssignmentInternalRequest message, Guid organisationId)
        => new(
            new BodyMandateId(message.BodyMandateId),
            new BodyId(message.Body.BodyId),
            new BodySeatId(message.Body.BodySeatId),
            new DelegationAssignmentId(message.Body.DelegationAssignmentId),
            new PersonId(message.Body.PersonId),
            new OrganisationId(organisationId),
            message.Body.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value) ?? new Dictionary<ContactTypeId, string>(),
            new Period(
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo)));
}
