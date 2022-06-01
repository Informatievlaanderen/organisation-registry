namespace OrganisationRegistry.Api.Backoffice.Organisation.Relation;

using System;
using OrganisationRegistry.SqlServer.Organisation;

public class OrganisationRelationResponse
{
    public Guid OrganisationRelationId { get; set; }
    public Guid OrganisationId { get; set; }

    public Guid RelationId { get; set; }
    public string RelationName { get; set; }

    public Guid RelatedOrganisationId { get; set; }
    public string RelatedOrganisationName { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public OrganisationRelationResponse(OrganisationRelationListItem organisationRelation)
    {
        OrganisationRelationId = organisationRelation.OrganisationRelationId;
        OrganisationId = organisationRelation.OrganisationId;

        RelationId = organisationRelation.RelationId;
        RelationName = organisationRelation.RelationName;

        RelatedOrganisationId = organisationRelation.RelatedOrganisationId;
        RelatedOrganisationName = organisationRelation.RelatedOrganisationName;

        ValidFrom = organisationRelation.ValidFrom;
        ValidTo = organisationRelation.ValidTo;
    }
}
