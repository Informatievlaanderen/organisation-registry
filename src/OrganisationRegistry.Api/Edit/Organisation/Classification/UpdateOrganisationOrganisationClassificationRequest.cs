namespace OrganisationRegistry.Api.Edit.Organisation.Classification;

using System;
using FluentValidation;
using OrganisationClassification;
using OrganisationClassificationType;
using OrganisationRegistry.Organisation;

public class UpdateOrganisationOrganisationClassificationInternalRequest
{
    public Guid OrganisationId { get; set; }
    public Guid OrganisationOrganisationClassificationId { get; set; }
    public UpdateOrganisationOrganisationClassificationRequest Body { get; }

    public UpdateOrganisationOrganisationClassificationInternalRequest(Guid organisationId, Guid organisationOrganisationClassificationId, UpdateOrganisationOrganisationClassificationRequest message)
    {
        OrganisationId = organisationId;
        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
        Body = message;
    }
}

public class UpdateOrganisationOrganisationClassificationRequest
{
    public Guid OrganisationClassificationTypeId { get; set; }
    public Guid OrganisationClassificationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationOrganisationClassificationInternalRequestValidator : AbstractValidator<UpdateOrganisationOrganisationClassificationInternalRequest>
{
    public UpdateOrganisationOrganisationClassificationInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.OrganisationClassificationTypeId)
            .NotEmpty()
            .WithMessage("Organisation Classification Type Id is required.");

        RuleFor(x => x.Body.OrganisationClassificationId)
            .NotEmpty()
            .WithMessage("Organisation Classification Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");

        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");
    }
}

public static class UpdateOrganisationOrganisationClassificationRequestMapping
{
    public static UpdateOrganisationOrganisationClassification Map(UpdateOrganisationOrganisationClassificationInternalRequest message)
        => new(
            message.OrganisationOrganisationClassificationId,
            new OrganisationId(message.OrganisationId),
            new OrganisationClassificationTypeId(message.Body.OrganisationClassificationTypeId),
            new OrganisationClassificationId(message.Body.OrganisationClassificationId),
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
