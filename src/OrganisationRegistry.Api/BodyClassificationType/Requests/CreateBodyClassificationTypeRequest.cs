namespace OrganisationRegistry.Api.BodyClassificationType.Requests
{
    using FluentValidation;
    using SqlServer.BodyClassificationType;
    using System;
    using OrganisationRegistry.BodyClassificationType;
    using OrganisationRegistry.BodyClassificationType.Commands;

    public class CreateBodyClassificationTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateBodyClassificationTypeRequestValidator : AbstractValidator<CreateBodyClassificationTypeRequest>
    {
        public CreateBodyClassificationTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, BodyClassificationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BodyClassificationTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateBodyClassificationTypeRequestMapping
    {
        public static CreateBodyClassificationType Map(CreateBodyClassificationTypeRequest message)
        {
            return new CreateBodyClassificationType(
                new BodyClassificationTypeId(message.Id),
                message.Name);
        }
    }
}
