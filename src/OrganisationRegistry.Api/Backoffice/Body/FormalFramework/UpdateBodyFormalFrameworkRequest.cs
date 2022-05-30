namespace OrganisationRegistry.Api.Backoffice.Body.FormalFramework;

using System;
using FluentValidation;
using OrganisationRegistry.Body;
using OrganisationRegistry.FormalFramework;

public class UpdateBodyFormalFrameworkInternalRequest
{
    public Guid BodyId { get; }
    public UpdateBodyFormalFrameworkRequest Body { get; }

    public UpdateBodyFormalFrameworkInternalRequest(Guid bodyId, UpdateBodyFormalFrameworkRequest message)
    {
        BodyId = bodyId;
        Body = message;
    }
}

public class UpdateBodyFormalFrameworkRequest
{
    public Guid BodyFormalFrameworkId { get; set; }
    public Guid FormalFrameworkId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateBodyFormalFrameworkInternalRequestValidator : AbstractValidator<UpdateBodyFormalFrameworkInternalRequest>
{
    public UpdateBodyFormalFrameworkInternalRequestValidator()
    {
        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.BodyFormalFrameworkId)
            .NotEmpty()
            .WithMessage("Body Formal Framework Id is required.");

        RuleFor(x => x.Body.FormalFrameworkId)
            .NotEmpty()
            .WithMessage("Formal Framework Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateBodyFormalFrameworkRequestMapping
{
    public static UpdateBodyFormalFramework Map(UpdateBodyFormalFrameworkInternalRequest message)
    {
        return new UpdateBodyFormalFramework(
            new BodyId(message.BodyId),
            new BodyFormalFrameworkId(message.Body.BodyFormalFrameworkId),
            new FormalFrameworkId(message.Body.FormalFrameworkId),
            new Period(
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo)));
    }
}