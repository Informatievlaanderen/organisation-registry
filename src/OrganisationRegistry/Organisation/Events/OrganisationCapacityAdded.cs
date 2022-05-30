namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OrganisationCapacityAdded : BaseEvent<OrganisationCapacityAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationCapacityId { get; }
    public Guid CapacityId { get; }
    public string CapacityName { get; }
    public Guid? PersonId { get; }
    public string PersonFullName { get; }
    public Guid? FunctionId { get; }
    public string FunctionName { get; }
    public Guid? LocationId { get; }
    public string LocationName { get; }
    public Dictionary<Guid, string> Contacts { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    [JsonConstructor]
    public OrganisationCapacityAdded(
        Guid organisationId,
        Guid organisationCapacityId,
        Guid capacityId,
        string capacityName,
        Guid? personId,
        string personFullName,
        Guid? functionId,
        string functionName,
        Guid? locationId,
        string locationName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationCapacityId = organisationCapacityId;
        CapacityId = capacityId;
        CapacityName = capacityName;
        PersonId = personId;
        PersonFullName = personFullName;
        FunctionId = functionId;
        FunctionName = functionName;
        LocationId = locationId;
        LocationName = locationName;
        Contacts = contacts;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}