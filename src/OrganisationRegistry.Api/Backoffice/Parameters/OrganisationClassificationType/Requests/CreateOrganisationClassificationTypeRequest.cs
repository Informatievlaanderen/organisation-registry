namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassificationType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.OrganisationClassificationType;
using OrganisationRegistry.OrganisationClassificationType.Commands;
using SqlServer.OrganisationClassificationType;

public class CreateOrganisationClassificationTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public bool AllowDifferentClassificationsToOverlap { get; set; } = false;
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
        => new(
            new OrganisationClassificationTypeId(message.Id),
            new OrganisationClassificationTypeName(message.Name),
            message.AllowDifferentClassificationsToOverlap
        );
}
