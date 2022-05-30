namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.Body;

public class RemoveDelegationAssignmentInternalRequest
{
    public Guid BodyMandateId { get; set; }
    public RemoveDelegationAssignmentRequest Body { get; }

    public RemoveDelegationAssignmentInternalRequest(Guid bodyMandateId, RemoveDelegationAssignmentRequest message)
    {
        BodyMandateId = bodyMandateId;
        Body = message;
    }
}

public class RemoveDelegationAssignmentRequest
{
    public Guid DelegationAssignmentId { get; set; }
    public Guid BodyId { get; set; }
    public Guid BodySeatId { get; set; }
}

public class RemoveDelegationAssignmentInternalRequestValidator : AbstractValidator<RemoveDelegationAssignmentInternalRequest>
{
    public RemoveDelegationAssignmentInternalRequestValidator()
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
    }
}

public static class RemoveDelegationAssignmentRequestMapping
{
    public static RemoveDelegationAssignment Map(RemoveDelegationAssignmentInternalRequest message)
    {
        return new RemoveDelegationAssignment(
            new BodyMandateId(message.BodyMandateId),
            new BodyId(message.Body.BodyId),
            new BodySeatId(message.Body.BodySeatId),
            new DelegationAssignmentId(message.Body.DelegationAssignmentId));
    }
}