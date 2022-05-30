namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationContactUpdated : BaseEvent<OrganisationContactUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationContactId { get; }

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

    public OrganisationContactUpdated(
        Guid organisationId,
        Guid organisationContactId,
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
        Id = organisationId;

        OrganisationContactId = organisationContactId;
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