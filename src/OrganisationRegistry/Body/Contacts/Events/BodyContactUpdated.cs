namespace OrganisationRegistry.Body.Events;

using System;

public class BodyContactUpdated : BaseEvent<BodyContactUpdated>
{
    public Guid BodyId => Id;

    public Guid BodyContactId { get; }

    public Guid ContactTypeId { get; }
    public Guid PreviousContactTypeId { get; }

    public string ContactTypeName { get; }
    public string PreviousContactTypeName { get; }

    public string Value { get; }
    public string PreviousValue { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public BodyContactUpdated(
        Guid bodyId,
        Guid bodyContactId,
        Guid contactTypeId,
        string contactTypeName,
        string value,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousContactTypeId,
        string previousContactTypeName,
        string previousValue,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodyContactId = bodyContactId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousContactTypeId = previousContactTypeId;
        PreviousContactTypeName = previousContactTypeName;
        PreviousValue = previousValue;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}