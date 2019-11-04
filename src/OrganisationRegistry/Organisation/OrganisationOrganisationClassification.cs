namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationOrganisationClassification
    {
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public Guid OrganisationOrganisationClassificationId { get; }
        public Guid OrganisationClassificationTypeId { get; }
        public string OrganisationClassificationTypeName { get; }
        public Guid OrganisationClassificationId { get; }
        public string OrganisationClassificationName { get; }
        public Period Validity { get; }

        public OrganisationOrganisationClassification(
            Guid organisationOrganisationClassificationId,
            Guid organisationId,
            Guid organisationClassificationTypeId,
            string organisationClassificationTypeName,
            Guid organisationClassificationId,
            string organisationClassificationName,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
            OrganisationClassificationTypeId = organisationClassificationTypeId;
            OrganisationClassificationId = organisationClassificationId;
            Validity = validity;
            OrganisationClassificationTypeName = organisationClassificationTypeName;
            OrganisationClassificationName = organisationClassificationName;
        }
    }
}
