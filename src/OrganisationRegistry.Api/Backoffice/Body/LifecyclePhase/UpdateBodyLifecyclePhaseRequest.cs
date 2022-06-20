namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase;

using System;
using FluentValidation;
using OrganisationRegistry.Body;
using LifecyclePhaseType;

public class UpdateBodyLifecyclePhaseInternalRequest
{
    public Guid BodyId { get; }
    public UpdateBodyLifecyclePhaseRequest Body { get; }

    public UpdateBodyLifecyclePhaseInternalRequest(Guid bodyId, UpdateBodyLifecyclePhaseRequest message)
    {
        BodyId = bodyId;
        Body = message;
    }
}

public class UpdateBodyLifecyclePhaseRequest
{
    public Guid BodyLifecyclePhaseId { get; set; }
    public Guid LifecyclePhaseTypeId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateBodyLifecyclePhaseInternalRequestValidator : AbstractValidator<UpdateBodyLifecyclePhaseInternalRequest>
{
    public UpdateBodyLifecyclePhaseInternalRequestValidator()
    {
        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.BodyLifecyclePhaseId)
            .NotEmpty()
            .WithMessage("Body Lifecycle Phase Id is required.");

        RuleFor(x => x.Body.LifecyclePhaseTypeId)
            .NotEmpty()
            .WithMessage("Lifecycle Phase Type Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateBodyLifecyclePhaseRequestMapping
{
    public static UpdateBodyLifecyclePhase Map(UpdateBodyLifecyclePhaseInternalRequest message)
        => new(
            new BodyId(message.BodyId),
            new BodyLifecyclePhaseId(message.Body.BodyLifecyclePhaseId),
            new LifecyclePhaseTypeId(message.Body.LifecyclePhaseTypeId),
            new Period(
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo)));
}
