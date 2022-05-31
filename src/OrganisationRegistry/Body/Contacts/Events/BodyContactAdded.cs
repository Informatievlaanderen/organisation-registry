namespace OrganisationRegistry.Body.Events;

using System;

public class BodyContactAdded : BaseEvent<BodyContactAdded>
{
    public Guid BodyId => Id;

    public Guid BodyContactId { get; }
    public Guid ContactTypeId { get; }
    public string ContactTypeName { get; }
    public string Value { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public BodyContactAdded(
        Guid bodyId,
        Guid bodyContactId,
        Guid contactTypeId,
        string contactTypeName,
        string value,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyContactId = bodyContactId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
