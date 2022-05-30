namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationContactAdded : BaseEvent<OrganisationContactAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationContactId { get; }
    public Guid ContactTypeId { get; }
    public string ContactTypeName { get; }
    public string Value { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationContactAdded(
        Guid organisationId,
        Guid organisationContactId,
        Guid contactTypeId,
        string contactTypeName,
        string value,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationContactId = organisationContactId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}