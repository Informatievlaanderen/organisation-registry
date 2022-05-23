namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.OrganisationRelationType;
    using OrganisationRegistry.OrganisationRelationType.Commands;
    using SqlServer.OrganisationRelationType;

    public class CreateOrganisationRelationTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? InverseName { get; set; }
    }

    public class CreateOrganisationRelationTypeRequestValidator : AbstractValidator<CreateOrganisationRelationTypeRequest>
    {
        public CreateOrganisationRelationTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, OrganisationRelationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationRelationTypeListConfiguration.NameLength}.");

            RuleFor(x => x.InverseName)
                .Length(0, OrganisationRelationTypeListConfiguration.NameLength)
                .WithMessage($"Inverse Name cannot be longer than {OrganisationRelationTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateOrganisationRelationTypeRequestMapping
    {
        public static CreateOrganisationRelationType Map(CreateOrganisationRelationTypeRequest message)
            => new(
                new OrganisationRelationTypeId(message.Id),
                message.Name,
                message.InverseName);
    }
}
