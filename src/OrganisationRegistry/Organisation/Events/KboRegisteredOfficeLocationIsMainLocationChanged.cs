namespace OrganisationRegistry.Organisation.Events;

using System;

public class KboRegisteredOfficeLocationIsMainLocationChanged : BaseEvent<KboRegisteredOfficeLocationIsMainLocationChanged>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationLocationId { get; }
    public bool IsMainLocation { get; }

    public KboRegisteredOfficeLocationIsMainLocationChanged(Guid organisationId, Guid organisationLocationId, bool isMainLocation)
    {
        Id = organisationId;
        OrganisationLocationId = organisationLocationId;
        IsMainLocation = isMainLocation;
    }
}
