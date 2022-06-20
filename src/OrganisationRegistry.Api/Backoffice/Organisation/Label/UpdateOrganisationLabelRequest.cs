namespace OrganisationRegistry.Api.Backoffice.Organisation.Label;

using System;
using FluentValidation;
using LabelType;
using OrganisationRegistry.Organisation;
using SqlServer.Organisation;

public class UpdateOrganisationLabelInternalRequest
{
    public Guid OrganisationId { get; set; }
    public UpdateOrganisationLabelRequest Body { get; }

    public UpdateOrganisationLabelInternalRequest(Guid organisationId, UpdateOrganisationLabelRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class UpdateOrganisationLabelRequest
{
    public Guid OrganisationLabelId { get; set; }
    public Guid LabelTypeId { get; set; }
    public string LabelValue { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationLabelInternalRequestValidator : AbstractValidator<UpdateOrganisationLabelInternalRequest>
{
    public UpdateOrganisationLabelInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.LabelTypeId)
            .NotEmpty()
            .WithMessage("Label Type Id is required.");

        RuleFor(x => x.Body.LabelValue)
            .NotEmpty()
            .WithMessage("Label Value is required.");

        RuleFor(x => x.Body.LabelValue)
            .Length(0, OrganisationLabelListConfiguration.LabelValueLength)
            .WithMessage($"Label Value cannot be longer than {OrganisationLabelListConfiguration.LabelValueLength}.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");

        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");

        // TODO: Validate if org id is valid
    }
}

public static class UpdateOrganisationLabelRequestMapping
{
    public static UpdateOrganisationLabel Map(UpdateOrganisationLabelInternalRequest message)
        => new(
            message.Body.OrganisationLabelId,
            new OrganisationId(message.OrganisationId),
            new LabelTypeId(message.Body.LabelTypeId),
            message.Body.LabelValue,
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
