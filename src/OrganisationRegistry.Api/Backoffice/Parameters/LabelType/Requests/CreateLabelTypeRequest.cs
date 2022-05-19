namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.LabelType;
    using OrganisationRegistry.LabelType.Commands;
    using SqlServer.LabelType;

    public class CreateLabelTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class CreateLabelTypeRequestValidator : AbstractValidator<CreateLabelTypeRequest>
    {
        public CreateLabelTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, LabelTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LabelTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateLabelTypeRequestMapping
    {
        public static CreateLabelType Map(CreateLabelTypeRequest message)
            => new(
                new LabelTypeId(message.Id),
                new LabelTypeName(message.Name));
    }
}
