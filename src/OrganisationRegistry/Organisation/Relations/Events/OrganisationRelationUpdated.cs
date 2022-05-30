namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationRelationUpdated : BaseEvent<OrganisationRelationUpdated>
{
    public Guid OrganisationId => Id;

    public string OrganisationName { get; }

    public Guid OrganisationRelationId { get; }

    public Guid RelationId { get; }
    public Guid PreviousRelationId { get; }

    public string RelationName { get; }
    public string PreviousRelationName { get; }

    public string RelationInverseName { get; }
    public string PreviousRelationInverseName { get; }

    public Guid RelatedOrganisationId { get; }
    public Guid PreviousRelatedOrganisationId { get; }

    public string RelatedOrganisationName { get; }
    public string PreviousRelatedOrganisationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationRelationUpdated(
        Guid organisationId,
        string organisationName,
        Guid organisationRelationId,
        Guid relatedOrganisationId,
        string relatedOrganisationName,
        Guid relationId,
        string relationName,
        string relationInverseName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousRelatedOrganisationId,
        string previousRelatedOrganisationName,
        Guid previousRelationId,
        string previousRelationName,
        string previousRelationInverseName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationName = organisationName;
        OrganisationRelationId = organisationRelationId;

        RelatedOrganisationId = relatedOrganisationId;
        RelatedOrganisationName = relatedOrganisationName;
        RelationId = relationId;
        RelationName = relationName;
        RelationInverseName = relationInverseName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousRelatedOrganisationId = previousRelatedOrganisationId;
        PreviousRelatedOrganisationName = previousRelatedOrganisationName;
        PreviousRelationId = previousRelationId;
        PreviousRelationName = previousRelationName;
        PreviousRelationInverseName = previousRelationInverseName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}