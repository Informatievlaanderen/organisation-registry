namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Capacity;
using ContactType;
using Function;
using Location;
using Person;

public class UpdateOrganisationCapacity : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId
        => Id;

    public Guid OrganisationCapacityId { get; }
    public CapacityId CapacityId { get; }
    public PersonId? PersonId { get; }
    public FunctionTypeId? FunctionTypeId { get; }
    public LocationId? LocationId { get; }
    public Dictionary<ContactTypeId, string> Contacts { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationCapacity(
        Guid organisationCapacityId,
        OrganisationId organisationId,
        CapacityId capacityId,
        PersonId? personId,
        FunctionTypeId? functionTypeId,
        LocationId? locationId,
        Dictionary<ContactTypeId, string>? contacts,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationCapacityId = organisationCapacityId;
        CapacityId = capacityId;
        PersonId = personId;
        FunctionTypeId = functionTypeId;
        LocationId = locationId;
        Contacts = contacts ?? new Dictionary<ContactTypeId, string>();
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
