namespace OrganisationRegistry.Api.Backoffice.Body.BodyClassification;

using System;
using FluentValidation;
using OrganisationRegistry.Body;
using OrganisationRegistry.BodyClassification;
using BodyClassificationType;

public class UpdateBodyBodyClassificationInternalRequest
{
    public Guid BodyId { get; set; }
    public UpdateBodyBodyClassificationRequest Body { get; }

    public UpdateBodyBodyClassificationInternalRequest(Guid bodyId, UpdateBodyBodyClassificationRequest message)
    {
        BodyId = bodyId;
        Body = message;
    }
}

public class UpdateBodyBodyClassificationRequest
{
    public Guid BodyBodyClassificationId { get; set; }
    public Guid BodyClassificationTypeId { get; set; }
    public Guid BodyClassificationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateBodyBodyClassificationInternalRequestValidator : AbstractValidator<UpdateBodyBodyClassificationInternalRequest>
{
    public UpdateBodyBodyClassificationInternalRequestValidator()
    {
        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.BodyClassificationTypeId)
            .NotEmpty()
            .WithMessage("Body Classification Type Id is required.");

        RuleFor(x => x.Body.BodyClassificationId)
            .NotEmpty()
            .WithMessage("Body Classification Id is required.");

        // TODO: Validate if BodyClassificationTypeId is valid

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");

        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Body Id is required.");

        // TODO: Validate if body id is valid
    }
}

public static class UpdateBodyBodyClassificationRequestMapping
{
    public static UpdateBodyBodyClassification Map(UpdateBodyBodyClassificationInternalRequest message)
    {
        return new UpdateBodyBodyClassification(
            message.Body.BodyBodyClassificationId,
            new BodyId(message.BodyId),
            new BodyClassificationTypeId(message.Body.BodyClassificationTypeId),
            new BodyClassificationId(message.Body.BodyClassificationId),
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
    }
}