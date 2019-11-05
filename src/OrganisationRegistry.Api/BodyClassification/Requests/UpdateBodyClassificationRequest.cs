namespace OrganisationRegistry.Api.BodyClassification.Requests
{
    using FluentValidation;
    using SqlServer.BodyClassification;
    using System;
    using OrganisationRegistry.BodyClassification;
    using OrganisationRegistry.BodyClassification.Commands;
    using OrganisationRegistry.BodyClassificationType;

    public class UpdateBodyClassificationInternalRequest
    {
        public Guid BodyClassificationId { get; set; }
        public UpdateBodyClassificationRequest Body { get; set; }

        public UpdateBodyClassificationInternalRequest(Guid bodyClassificationId, UpdateBodyClassificationRequest body)
        {
            BodyClassificationId = bodyClassificationId;
            Body = body;
        }
    }

    public class UpdateBodyClassificationRequest
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }
        public Guid BodyClassificationTypeId { get; set; }
    }

    public class UpdateBodyClassificationRequestValidator : AbstractValidator<UpdateBodyClassificationInternalRequest>
    {
        public UpdateBodyClassificationRequestValidator()
        {
            RuleFor(x => x.BodyClassificationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, BodyClassificationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BodyClassificationListConfiguration.NameLength}.");

            RuleFor(x => x.Body.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0.");

            RuleFor(x => x.Body.BodyClassificationTypeId)
                .NotEmpty()
                .WithMessage("BodyClassificationTypeId is required.");
        }
    }

    public static class UpdateBodyClassificationRequestMapping
    {
        public static UpdateBodyClassification Map(UpdateBodyClassificationInternalRequest message)
        {
            return new UpdateBodyClassification(
                new BodyClassificationId(message.BodyClassificationId),
                message.Body.Name,
                message.Body.Order,
                message.Body.Active,
                new BodyClassificationTypeId(message.Body.BodyClassificationTypeId));
        }
    }
}
