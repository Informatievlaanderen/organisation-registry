namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OrganisationCapacityUpdated : BaseEvent<OrganisationCapacityUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationCapacityId { get; }

    public Guid CapacityId { get; }
    public Guid PreviousCapacityId { get; }

    public string CapacityName { get; }
    public string PreviousCapacityName { get; }

    public Guid? PersonId { get; }
    public Guid? PreviousPersonId { get; }

    public string PersonFullName { get; }
    public string PreviousPersonFullName { get; }

    public Guid? FunctionId { get; }
    public Guid? PreviousFunctionId { get; }

    public string FunctionName { get; }
    public string PreviousFunctionName { get; }

    public Guid? LocationId { get; }
    public Guid? PreviousLocationId { get; }

    public string LocationName { get; }
    public string PreviousLocationName { get; }

    public Dictionary<Guid, string> Contacts { get; }
    public Dictionary<Guid, string> PreviousContacts { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public bool PreviouslyActive { get; }

    [JsonConstructor]
    public OrganisationCapacityUpdated(
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
        DateTime? validTo,
        Guid previousCapacityId,
        string previousCapacityName,
        Guid? previousPersonId,
        string previousPersonFullName,
        Guid? previousFunctionId,
        string previousFunctionName,
        Guid? previousLocationId,
        string previousLocationName,
        Dictionary<Guid, string> previousContacts,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo,
        bool previouslyActive)
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

        PreviousCapacityId = previousCapacityId;
        PreviousCapacityName = previousCapacityName;
        PreviousPersonId = previousPersonId;
        PreviousPersonFullName = previousPersonFullName;
        PreviousFunctionId = previousFunctionId;
        PreviousFunctionName = previousFunctionName;
        PreviousLocationId = previousLocationId;
        PreviousLocationName = previousLocationName;
        PreviousContacts = previousContacts;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
        PreviouslyActive = previouslyActive;
    }
}
