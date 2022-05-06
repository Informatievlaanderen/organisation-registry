namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationShortNameUpdated : BaseEvent<OrganisationShortNameUpdated>
{
    public Guid OrganisationId => Id;

    public string ShortName { get; }

    public OrganisationShortNameUpdated(
        Guid organisationId,
        string shortName)
    {
        Id = organisationId;

        ShortName = shortName;
    }
}