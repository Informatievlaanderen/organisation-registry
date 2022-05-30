namespace OrganisationRegistry.OrganisationClassification.Events;

using System;
using Newtonsoft.Json;
using OrganisationClassificationType;

public class OrganisationClassificationCreated : BaseEvent<OrganisationClassificationCreated>
{
    public Guid OrganisationClassificationId => Id;

    public string Name { get; }
    public int Order { get; }
    public string? ExternalKey { get; }
    public bool Active { get; }
    public Guid OrganisationClassificationTypeId { get; }
    public string OrganisationClassificationTypeName { get; }

    public OrganisationClassificationCreated(
        OrganisationClassificationId organisationClassificationId,
        OrganisationClassificationName name,
        int order,
        string? externalKey,
        bool active,
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName organisationClassificationTypeName)
    {
        Id = organisationClassificationId;

        Name = name;
        Order = order;
        ExternalKey = externalKey;
        Active = active;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationTypeName = organisationClassificationTypeName;
    }

    [JsonConstructor]
    public OrganisationClassificationCreated(
        Guid organisationClassificationId,
        string name,
        int order,
        string? externalKey,
        bool active,
        Guid organisationClassificationTypeId,
        string organisationClassificationTypeName)
        : this(
            new OrganisationClassificationId(organisationClassificationId),
            new OrganisationClassificationName(name),
            order,
            externalKey,
            active,
            new OrganisationClassificationTypeId(organisationClassificationTypeId),
            new OrganisationClassificationTypeName(organisationClassificationTypeName)) { }
}