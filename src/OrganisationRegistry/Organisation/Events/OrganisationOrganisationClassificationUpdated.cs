namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationOrganisationClassificationUpdated : BaseEvent<OrganisationOrganisationClassificationUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationOrganisationClassificationId { get; }

    public Guid OrganisationClassificationTypeId { get; }
    public Guid PreviousOrganisationClassificationTypeId { get; }

    public string OrganisationClassificationTypeName { get; }
    public string PreviousOrganisationClassificationTypeName { get; }

    public Guid OrganisationClassificationId { get; }
    public Guid PreviousOrganisationClassificationId { get; }

    public string OrganisationClassificationName { get; }
    public string PreviousOrganisationClassificationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationOrganisationClassificationUpdated(
        Guid organisationId,
        Guid organisationOrganisationClassificationId,
        Guid organisationClassificationTypeId,
        string organisationClassificationTypeName,
        Guid organisationClassificationId,
        string organisationClassificationName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousOrganisationClassificationTypeId,
        string previousOrganisationClassificationTypeName,
        Guid previousOrganisationClassificationId,
        string previousOrganisationClassificationName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationTypeName = organisationClassificationTypeName;
        OrganisationClassificationId = organisationClassificationId;
        OrganisationClassificationName = organisationClassificationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousOrganisationClassificationTypeId = previousOrganisationClassificationTypeId;
        PreviousOrganisationClassificationTypeName = previousOrganisationClassificationTypeName;
        PreviousOrganisationClassificationId = previousOrganisationClassificationId;
        PreviousOrganisationClassificationName = previousOrganisationClassificationName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}