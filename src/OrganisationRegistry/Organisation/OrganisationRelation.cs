namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationRelation
    {
        public Guid OrganisationRelationId { get; }
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public string OrganisationName { get; }
        public Guid RelationId { get; }
        public string RelationName { get; }
        public string RelationInverseName { get; }
        public Guid RelatedOrganisationId { get; }
        public string RelatedOrganisationName { get; }
        public Period Validity { get; }

        public OrganisationRelation(
            Guid organisationRelationId,
            Guid organisationId,
            string organisationName,
            Guid relationId,
            string relationName,
            string relationInverseName,
            Guid relatedOrganisationId,
            string relatedOrganisationName,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            OrganisationRelationId = organisationRelationId;
            RelationId = relationId;
            RelationName = relationName;
            RelationInverseName = relationInverseName;
            RelatedOrganisationId = relatedOrganisationId;
            RelatedOrganisationName = relatedOrganisationName;
            Validity = validity;
        }
    }
}
