namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationValidityUpdated : BaseEvent<OrganisationValidityUpdated>
{
    public Guid OrganisationId => Id;

    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationValidityUpdated(
        Guid organisationId,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}