namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationCapacityBecameInactive: BaseEvent<OrganisationCapacityBecameInactive>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationCapacityId { get; }
    public Guid CapacityId { get; }
    public Guid? PersonId { get; }
    public Guid? FunctionTypeId { get; }
    public DateTime? EndOfValidity { get; }

    public OrganisationCapacityBecameInactive(
        Guid organisationId,
        Guid organisationCapacityId,
        Guid capacityId,
        Guid? personId,
        Guid? functionTypeId,
        DateTime? endOfValidity)
    {
        Id = organisationId;

        OrganisationCapacityId = organisationCapacityId;
        CapacityId = capacityId;
        PersonId = personId;
        FunctionTypeId = functionTypeId;
        EndOfValidity = endOfValidity;
    }
}
