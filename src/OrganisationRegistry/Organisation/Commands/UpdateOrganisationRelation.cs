namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using OrganisationRelationType;

    public class UpdateOrganisationRelation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationRelationId { get; }
        public OrganisationRelationTypeId RelationTypeId { get; }
        public OrganisationId RelatedOrganisationId { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public UpdateOrganisationRelation(
            Guid organisationRelationId,
            OrganisationRelationTypeId relationTypeId,
            OrganisationId organisationId,
            OrganisationId relatedOrganisationId,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationRelationId = organisationRelationId;
            RelatedOrganisationId = relatedOrganisationId;
            RelationTypeId = relationTypeId;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
