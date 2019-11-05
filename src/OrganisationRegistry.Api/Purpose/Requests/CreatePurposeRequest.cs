namespace OrganisationRegistry.Api.Purpose.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Purpose;
    using OrganisationRegistry.Purpose;
    using OrganisationRegistry.Purpose.Commands;

    public class CreatePurposeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreatePurposeRequestValidator : AbstractValidator<CreatePurposeRequest>
    {
        public CreatePurposeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, PurposeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {PurposeListConfiguration.NameLength}.");
        }
    }

    public static class CreatePurposeRequestMapping
    {
        public static CreatePurpose Map(CreatePurposeRequest message)
        {
            return new CreatePurpose(
                new PurposeId(message.Id),
                new PurposeName(message.Name));
        }
    }
}
