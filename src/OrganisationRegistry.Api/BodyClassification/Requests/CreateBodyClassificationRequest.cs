namespace OrganisationRegistry.Api.BodyClassification.Requests
{
    using FluentValidation;
    using SqlServer.BodyClassification;
    using System;
    using OrganisationRegistry.BodyClassification;
    using OrganisationRegistry.BodyClassification.Commands;
    using OrganisationRegistry.BodyClassificationType;

    public class CreateBodyClassificationRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public Guid BodyClassificationTypeId { get; set; }
    }

    public class CreateBodyClassificationRequestValidator : AbstractValidator<CreateBodyClassificationRequest>
    {
        public CreateBodyClassificationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, BodyClassificationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BodyClassificationListConfiguration.NameLength}.");

            RuleFor(x => x.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0.");

            RuleFor(x => x.BodyClassificationTypeId)
                .NotEmpty()
                .WithMessage("BodyClassificationTypeId is required.");
        }
    }

    public static class CreateBodyClassificationRequestMapping
    {
        public static CreateBodyClassification Map(CreateBodyClassificationRequest message)
        {
            return new CreateBodyClassification(
                new BodyClassificationId(message.Id),
                message.Name,
                message.Order,
                message.Active,
                new BodyClassificationTypeId(message.BodyClassificationTypeId));
        }
    }
}
