namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationRelationAdded : BaseEvent<OrganisationRelationAdded>
{
    public Guid OrganisationId => Id;

    public string OrganisationName { get; }
    public Guid OrganisationRelationId { get; }
    public Guid RelationId { get; }
    public string RelationName { get; }
    public string RelationInverseName { get; set; }
    public Guid RelatedOrganisationId { get; }
    public string RelatedOrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationRelationAdded(
        Guid organisationId,
        string organisationName,
        Guid organisationRelationId,
        Guid relatedOrganisationId,
        string relatedOrganisationName,
        Guid relationId,
        string relationName,
        string relationInverseName,
        DateTime? validFrom,
        DateTime? validTo)
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
    }
}