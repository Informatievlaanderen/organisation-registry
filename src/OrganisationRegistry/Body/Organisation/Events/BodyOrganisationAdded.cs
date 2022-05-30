namespace OrganisationRegistry.Body.Events;

using System;

public class BodyOrganisationAdded : BaseEvent<BodyOrganisationAdded>
{
    public Guid BodyId => Id;

    public Guid BodyOrganisationId { get; }
    public Guid OrganisationId { get; }
    public string BodyName { get; }
    public string OrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public BodyOrganisationAdded(
        Guid bodyId,
        Guid bodyOrganisationId,
        string bodyName,
        Guid organisationId,
        string organisationName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyOrganisationId = bodyOrganisationId;
        BodyName = bodyName;
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}