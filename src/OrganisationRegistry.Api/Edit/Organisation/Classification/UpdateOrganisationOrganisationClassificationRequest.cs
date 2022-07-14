﻿namespace OrganisationRegistry.Api.Edit.Organisation.Classification;

using System;
using FluentValidation;
using OrganisationClassification;
using OrganisationClassificationType;
using OrganisationRegistry.Organisation;

public class UpdateOrganisationOrganisationClassificationRequest
{
    public Guid OrganisationClassificationTypeId { get; set; }
    public Guid OrganisationClassificationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationOrganisationClassificationInternalRequestValidator : AbstractValidator<UpdateOrganisationOrganisationClassificationRequest>
{
    public UpdateOrganisationOrganisationClassificationInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationClassificationTypeId)
            .NotEmpty()
            .WithMessage("Organisation Classification Type Id is required.");

        RuleFor(x => x.OrganisationClassificationId)
            .NotEmpty()
            .WithMessage("Organisation Classification Id is required.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateOrganisationOrganisationClassificationRequestMapping
{
    public static UpdateOrganisationOrganisationClassification Map(Guid organisationId, Guid organisationOrganisationClassificationId, UpdateOrganisationOrganisationClassificationRequest message)
        => new(
            organisationOrganisationClassificationId,
            new OrganisationId(organisationId),
            new OrganisationClassificationTypeId(message.OrganisationClassificationTypeId),
            new OrganisationClassificationId(message.OrganisationClassificationId),
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
}
