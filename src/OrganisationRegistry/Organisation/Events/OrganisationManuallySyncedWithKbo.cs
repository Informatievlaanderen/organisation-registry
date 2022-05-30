namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationManuallySyncedWithKbo : BaseEvent<OrganisationManuallySyncedWithKbo>
{
    public Guid OrganisationId => Id;

    public OrganisationManuallySyncedWithKbo(
        Guid organisationId)
    {
        Id = organisationId;
    }
}