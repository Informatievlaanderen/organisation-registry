namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationSyncedFromKbo : BaseEvent<OrganisationSyncedFromKbo>
{
    public Guid OrganisationId => Id;
    public Guid? KBOSyncItemId { get; }

    public OrganisationSyncedFromKbo(
        Guid organisationId,
        Guid? kboSyncItemId)
    {
        Id = organisationId;
        KBOSyncItemId = kboSyncItemId;
    }
}
