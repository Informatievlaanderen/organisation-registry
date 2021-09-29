namespace OrganisationRegistry.Api.Backoffice.Body.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.BodyClassification;
    using OrganisationRegistry.BodyClassificationType;

    public class AddBodyBodyClassificationInternalRequest
    {
        public Guid BodyId { get; set; }
        public AddBodyBodyClassificationRequest Body { get; }

        public AddBodyBodyClassificationInternalRequest(Guid bodyId, AddBodyBodyClassificationRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class AddBodyBodyClassificationRequest
    {
        public Guid BodyBodyClassificationId { get; set; }
        public Guid BodyClassificationTypeId { get; set; }
        public Guid BodyClassificationId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddBodyBodyClassificationInternalRequestValidator : AbstractValidator<AddBodyBodyClassificationInternalRequest>
    {
        public AddBodyBodyClassificationInternalRequestValidator()
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

    public static class AddBodyBodyClassificationRequestMapping
    {
        public static AddBodyBodyClassification Map(AddBodyBodyClassificationInternalRequest message)
        {
            return new AddBodyBodyClassification(
                message.Body.BodyBodyClassificationId,
                new BodyId(message.BodyId),
                new BodyClassificationTypeId(message.Body.BodyClassificationTypeId),
                new BodyClassificationId(message.Body.BodyClassificationId),
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
