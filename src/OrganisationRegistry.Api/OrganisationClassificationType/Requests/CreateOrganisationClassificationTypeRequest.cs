namespace OrganisationRegistry.Api.OrganisationClassificationType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.OrganisationClassificationType;
    using OrganisationRegistry.OrganisationClassificationType;
    using OrganisationRegistry.OrganisationClassificationType.Commands;

    public class CreateOrganisationClassificationTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateOrganisationClassificationTypeRequestValidator : AbstractValidator<CreateOrganisationClassificationTypeRequest>
    {
        public CreateOrganisationClassificationTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, OrganisationClassificationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationClassificationTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateOrganisationClassificationTypeRequestMapping
    {
        public static CreateOrganisationClassificationType Map(CreateOrganisationClassificationTypeRequest message)
        {
            return new CreateOrganisationClassificationType(
                new OrganisationClassificationTypeId(message.Id),
                message.Name);
        }
    }
}
