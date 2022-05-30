namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.OrganisationRelationType;
using OrganisationRegistry.OrganisationRelationType.Commands;
using SqlServer.OrganisationRelationType;

public class UpdateOrganisationRelationTypeInternalRequest
{
    public Guid OrganisationRelationTypeId { get; set; }
    public UpdateOrganisationRelationTypeRequest Body { get; set; }

    public UpdateOrganisationRelationTypeInternalRequest(Guid organisationRelationTypeId, UpdateOrganisationRelationTypeRequest body)
    {
        OrganisationRelationTypeId = organisationRelationTypeId;
        Body = body;
    }
}

public class UpdateOrganisationRelationTypeRequest
{
    public string Name { get; set; } = null!;
    public string? InverseName { get; set; }
}

public class UpdateOrganisationRelationTypeRequestValidator : AbstractValidator<UpdateOrganisationRelationTypeInternalRequest>
{
    public UpdateOrganisationRelationTypeRequestValidator()
    {
        RuleFor(x => x.OrganisationRelationTypeId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, OrganisationRelationTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {OrganisationRelationTypeListConfiguration.NameLength}.");

        RuleFor(x => x.Body.InverseName)
            .Length(0, OrganisationRelationTypeListConfiguration.NameLength)
            .WithMessage($"Inverse Name cannot be longer than {OrganisationRelationTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateOrganisationRelationTypeRequestMapping
{
    public static UpdateOrganisationRelationType Map(UpdateOrganisationRelationTypeInternalRequest message)
        => new(
            new OrganisationRelationTypeId(message.OrganisationRelationTypeId),
            message.Body.Name,
            message.Body.InverseName);
}