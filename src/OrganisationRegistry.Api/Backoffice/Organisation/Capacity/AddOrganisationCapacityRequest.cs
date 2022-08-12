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

public class AddOrganisationCapacityRequest
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

public class AddOrganisationCapacityInternalRequestValidator : AbstractValidator<AddOrganisationCapacityRequest>
{
    public AddOrganisationCapacityInternalRequestValidator()
    {
        RuleFor(x => x.CapacityId)
            .NotEmpty()
            .WithMessage("Capacity Id is required.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class AddOrganisationCapacityRequestMapping
{
    public static AddOrganisationCapacity Map(Guid organisationId, AddOrganisationCapacityRequest message)
    {
        return new AddOrganisationCapacity(
            message.OrganisationCapacityId,
            new OrganisationId(organisationId),
            new CapacityId(message.CapacityId),
            message.PersonId.HasValue ? new PersonId(message.PersonId.Value) : null,
            message.FunctionId.HasValue ? new FunctionTypeId(message.FunctionId.Value) : null,
            message.LocationId.HasValue ? new LocationId(message.LocationId.Value) : null,
            message.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value),
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
    }
}
