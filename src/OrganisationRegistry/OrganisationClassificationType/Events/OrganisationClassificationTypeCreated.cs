namespace OrganisationRegistry.OrganisationClassificationType.Events;

using System;
using Newtonsoft.Json;

public class OrganisationClassificationTypeCreated : BaseEvent<OrganisationClassificationTypeCreated>
{
    public Guid OrganisationClassificationTypeId => Id;

    public string Name { get; }

    public OrganisationClassificationTypeCreated(
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName name)
    {
        Id = organisationClassificationTypeId;

        Name = name;
    }

    [JsonConstructor]
    public OrganisationClassificationTypeCreated(
        Guid organisationClassificationTypeId,
        string name)
        : this(
            new OrganisationClassificationTypeId(organisationClassificationTypeId),
            new OrganisationClassificationTypeName(name)) { }
}
