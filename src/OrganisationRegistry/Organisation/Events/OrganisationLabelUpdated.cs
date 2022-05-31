namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationLabelUpdated : BaseEvent<OrganisationLabelUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationLabelId { get; }

    public Guid LabelTypeId { get; }
    public Guid PreviousLabelTypeId { get; }

    public string LabelTypeName { get; }
    public string PreviousLabelTypeName { get; }

    public string Value { get; }
    public string PreviousValue { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationLabelUpdated(
        Guid organisationId,
        Guid organisationLabelId,
        Guid labelTypeId,
        string labelTypeName,
        string value,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousLabelTypeId,
        string previousLabelTypeName,
        string previousValue,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationLabelId = organisationLabelId;
        LabelTypeId = labelTypeId;
        LabelTypeName = labelTypeName;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousLabelTypeId = previousLabelTypeId;
        PreviousLabelTypeName = previousLabelTypeName;
        PreviousValue = previousValue;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
