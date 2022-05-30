namespace OrganisationRegistry.Api.Backoffice.Organisation.Location;

using System;
using FluentValidation;
using LocationType;
using OrganisationRegistry.Location;
using OrganisationRegistry.Organisation;

public class AddOrganisationLocationInternalRequest
{
    public Guid OrganisationId { get; set; }
    public AddOrganisationLocationRequest Body { get; }

    public AddOrganisationLocationInternalRequest(Guid organisationId, AddOrganisationLocationRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class AddOrganisationLocationRequest
{
    public Guid OrganisationLocationId { get; set; }
    public Guid LocationId { get; set; }
    public bool IsMainLocation { get; set; }
    public Guid? LocationTypeId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class AddOrganisationLocationInternalRequestValidator : AbstractValidator<AddOrganisationLocationInternalRequest>
{
    public AddOrganisationLocationInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.LocationId)
            .NotEmpty()
            .WithMessage("Location Id is required.");

        // TODO: Validate if LocationId is valid

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

public static class AddOrganisationLocationRequestMapping
{
    public static AddOrganisationLocation Map(AddOrganisationLocationInternalRequest message)
    {
        return new AddOrganisationLocation(
            message.Body.OrganisationLocationId,
            new OrganisationId(message.OrganisationId),
            new LocationId(message.Body.LocationId),
            message.Body.IsMainLocation,
            message.Body.LocationTypeId.HasValue ? new LocationTypeId(message.Body.LocationTypeId.Value) : null,
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
    }
}