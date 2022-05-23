namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.OrganisationClassification;
    using OrganisationRegistry.OrganisationClassification.Commands;
    using OrganisationRegistry.OrganisationClassificationType;
    using SqlServer.OrganisationClassification;

    public class CreateOrganisationClassificationRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int Order { get; set; }

        public string? ExternalKey { get; set; }

        public bool Active { get; set; }

        public Guid OrganisationClassificationTypeId { get; set; }
    }

    public class CreateOrganisationClassificationRequestValidator : AbstractValidator<CreateOrganisationClassificationRequest>
    {
        public CreateOrganisationClassificationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, OrganisationClassificationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationClassificationListConfiguration.NameLength}.");

            RuleFor(x => x.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0.");

            RuleFor(x => x.OrganisationClassificationTypeId)
                .NotEmpty()
                .WithMessage("OrganisationClassificationTypeId is required.");
        }
    }

    public static class CreateOrganisationClassificationRequestMapping
    {
        public static CreateOrganisationClassification Map(CreateOrganisationClassificationRequest message)
            => new(
                new OrganisationClassificationId(message.Id),
                new OrganisationClassificationName(message.Name),
                message.Order,
                message.ExternalKey,
                message.Active,
                new OrganisationClassificationTypeId(message.OrganisationClassificationTypeId));
    }
}
