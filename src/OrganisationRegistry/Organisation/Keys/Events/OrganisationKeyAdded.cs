namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationKeyAdded : BaseEvent<OrganisationKeyAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationKeyId { get; }
    public Guid KeyTypeId { get; }
    public string KeyTypeName { get; }
    public string Value { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationKeyAdded(
        Guid organisationId,
        Guid organisationKeyId,
        Guid keyTypeId,
        string keyTypeName,
        string value,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationKeyId = organisationKeyId;
        KeyTypeId = keyTypeId;
        KeyTypeName = keyTypeName;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
