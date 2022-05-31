namespace OrganisationRegistry.OrganisationClassification.Events;

using System;
using Newtonsoft.Json;
using OrganisationClassificationType;

public class OrganisationClassificationUpdated : BaseEvent<OrganisationClassificationUpdated>
{
    public Guid OrganisationClassificationId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public int Order { get; }
    public int PreviousOrder { get; }

    public string? ExternalKey { get; }
    public string? PreviousExternalKey { get; }

    public bool Active { get; }
    public bool PreviousActive { get; }

    public Guid OrganisationClassificationTypeId { get; }
    public Guid PreviousOrganisationClassificationTypeId { get; }

    public string OrganisationClassificationTypeName { get; }
    public string PreviousOrganisationClassificationTypeName { get; }

    public OrganisationClassificationUpdated(
        OrganisationClassificationId organisationClassificationId,
        OrganisationClassificationName name,
        int order,
        string? externalKey,
        bool active,
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName organisationClassificationTypeName,
        OrganisationClassificationName previousName,
        int previousOrder,
        string? previousExternalKey,
        bool previousActive,
        OrganisationClassificationTypeId previousOrganisationClassificationTypeId,
        OrganisationClassificationTypeName previousOrganisationClassificationTypeName)
    {
        Id = organisationClassificationId;

        Name = name;
        Order = order;
        ExternalKey = externalKey;
        Active = active;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationTypeName = organisationClassificationTypeName;

        PreviousName = previousName;
        PreviousOrder = previousOrder;
        PreviousExternalKey = previousExternalKey;
        PreviousActive = previousActive;
        PreviousOrganisationClassificationTypeId = previousOrganisationClassificationTypeId;
        PreviousOrganisationClassificationTypeName = previousOrganisationClassificationTypeName;
    }

    [JsonConstructor]
    public OrganisationClassificationUpdated(
        Guid organisationClassificationId,
        string name,
        int order,
        string? externalKey,
        bool active,
        Guid organisationClassificationTypeId,
        string organisationClassificationTypeName,
        string previousName,
        int previousOrder,
        string? previousExternalKey,
        bool previousActive,
        Guid previousOrganisationClassificationTypeId,
        string previousOrganisationClassificationTypeName)
        : this(
            new OrganisationClassificationId(organisationClassificationId),
            new OrganisationClassificationName(name),
            order,
            externalKey,
            active,
            new OrganisationClassificationTypeId(organisationClassificationTypeId),
            new OrganisationClassificationTypeName(organisationClassificationTypeName),
            new OrganisationClassificationName(previousName),
            previousOrder,
            previousExternalKey,
            previousActive,
            new OrganisationClassificationTypeId(previousOrganisationClassificationTypeId),
            new OrganisationClassificationTypeName(previousOrganisationClassificationTypeName)) { }
}
