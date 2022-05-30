namespace OrganisationRegistry.OrganisationRelationType.Events;

using System;

public class OrganisationRelationTypeCreated : BaseEvent<OrganisationRelationTypeCreated>
{
    public Guid OrganisationRelationTypeId => Id;

    public string Name { get; }
    public string InverseName { get; set; }

    public OrganisationRelationTypeCreated(
        Guid organisationRelationTypeId,
        string name,
        string? inverseName)
    {
        Id = organisationRelationTypeId;

        Name = name;
        InverseName = inverseName ?? string.Empty;
    }
}