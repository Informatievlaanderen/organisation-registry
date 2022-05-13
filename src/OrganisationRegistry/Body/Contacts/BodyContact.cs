namespace OrganisationRegistry.Body;

using System;

public class BodyContact
{
    public Guid BodyId { get; }
    public Guid BodyContactId { get; }
    public Guid ContactTypeId { get; }
    public string ContactTypeName { get; }
    public string Value { get; }
    public Period Validity { get; }

    public BodyContact(
        Guid bodyContactId,
        Guid bodyId,
        Guid contactTypeId,
        string contactTypeName,
        string value,
        Period validity)
    {
        BodyId = bodyId;
        BodyContactId = bodyContactId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        Value = value;
        Validity = validity;
    }
}
