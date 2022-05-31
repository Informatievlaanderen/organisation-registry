namespace OrganisationRegistry.Organisation.Commands;

using System;

public class SyncOrganisationWithKbo : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public DateTime ModificationTime { get; }
    public Guid? KboSyncItemId { get; }

    public SyncOrganisationWithKbo(
        OrganisationId organisationId,
        DateTimeOffset modificationTime,
        Guid? kboSyncItemId)
    {
        ModificationTime = modificationTime.Date;
        Id = organisationId;
        KboSyncItemId = kboSyncItemId;
    }
}
