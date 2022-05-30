namespace OrganisationRegistry.Organisation.Events;

using System;

public class KboLegalFormOrganisationOrganisationClassificationRemoved : BaseEvent<KboLegalFormOrganisationOrganisationClassificationRemoved>
{
    public Guid OrganisationId => Id;
    public Guid OrganisationOrganisationClassificationId { get; }
    public Guid OrganisationClassificationTypeId { get; }
    public string OrganisationClassificationTypeName { get; }
    public Guid OrganisationClassificationId { get; }
    public string OrganisationClassificationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public KboLegalFormOrganisationOrganisationClassificationRemoved(
        Guid organisationId, Guid organisationOrganisationClassificationId,
        Guid organisationClassificationTypeId, string organisationClassificationTypeName,
        Guid organisationClassificationId, string organisationClassificationName,
        DateTime? validFrom, DateTime? validTo, DateTime? previouslyValidTo)
    {
        Id = organisationId;
        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationTypeName = organisationClassificationTypeName;
        OrganisationClassificationId = organisationClassificationId;
        OrganisationClassificationName = organisationClassificationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
        PreviouslyValidTo = previouslyValidTo;
    }
}