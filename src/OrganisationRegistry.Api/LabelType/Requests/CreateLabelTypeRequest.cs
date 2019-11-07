namespace OrganisationRegistry.Api.LabelType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.LabelType;
    using OrganisationRegistry.LabelType;
    using OrganisationRegistry.LabelType.Commands;

    public class CreateLabelTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
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
            => new CreateLabelType(
                new LabelTypeId(message.Id),
                new LabelTypeName(message.Name));
    }
}
