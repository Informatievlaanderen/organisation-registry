namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationNameUpdated : BaseEvent<OrganisationNameUpdated>
{
    public Guid OrganisationId => Id;

    public string Name { get; }

    public OrganisationNameUpdated(
        Guid organisationId,
        string name){
        Id = organisationId;

        Name = name;
    }
}
