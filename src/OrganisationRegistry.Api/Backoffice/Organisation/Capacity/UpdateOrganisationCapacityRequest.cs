namespace OrganisationRegistry.Api.Backoffice.Organisation.Capacity;

using System;
using System.Collections.Generic;
using System.Linq;
using ContactType;
using FluentValidation;
using OrganisationRegistry.Capacity;
using OrganisationRegistry.Function;
using OrganisationRegistry.Location;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Person;

public class UpdateOrganisationCapacityInternalRequest
{
    public Guid OrganisationId { get; set; }
    public UpdateOrganisationCapacityRequest Body { get; }

    public UpdateOrganisationCapacityInternalRequest(Guid organisationId, UpdateOrganisationCapacityRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class UpdateOrganisationCapacityRequest
{
    public Guid OrganisationCapacityId { get; set; }
    public Guid CapacityId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? FunctionId { get; set; }
    public Guid? LocationId { get; set; }
    public Dictionary<Guid, string>? Contacts { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationCapacityInternalRequestValidator : AbstractValidator<UpdateOrganisationCapacityInternalRequest>
{
    public UpdateOrganisationCapacityInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.CapacityId)
            .NotEmpty()
            .WithMessage("Capacity Id is required.");

        // TODO: Validate if CapacityTypeId is valid

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

public static class UpdateOrganisationCapacityRequestMapping
{
    public static UpdateOrganisationCapacity Map(UpdateOrganisationCapacityInternalRequest message)
        => new(
            message.Body.OrganisationCapacityId,
            new OrganisationId(message.OrganisationId),
            new CapacityId(message.Body.CapacityId),
            message.Body.PersonId.HasValue ? new PersonId(message.Body.PersonId.Value) : null,
            message.Body.FunctionId.HasValue ? new FunctionTypeId(message.Body.FunctionId.Value) : null,
            message.Body.LocationId.HasValue ? new LocationId(message.Body.LocationId.Value) : null,
            message.Body.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value),
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
