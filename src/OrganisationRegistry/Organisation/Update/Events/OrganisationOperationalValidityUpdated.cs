namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationOperationalValidityUpdated : BaseEvent<OrganisationOperationalValidityUpdated>
{
    public Guid OrganisationId
        => Id;

    public DateTime? OperationalValidFrom { get; }
    public DateTime? OperationalValidTo { get; }


    public OrganisationOperationalValidityUpdated(
        Guid organisationId,
        DateTime? operationalValidFrom,
        DateTime? operationalValidTo)
    {
        Id = organisationId;

        OperationalValidFrom = operationalValidFrom;
        OperationalValidTo = operationalValidTo;
    }
}
