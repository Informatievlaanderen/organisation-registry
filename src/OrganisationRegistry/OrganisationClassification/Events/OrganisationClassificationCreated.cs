namespace OrganisationRegistry.OrganisationClassification.Events
{
    using System;

    public class OrganisationClassificationCreated : BaseEvent<OrganisationClassificationCreated>
    {
        public Guid OrganisationClassificationId => Id;

        public string Name { get; }
        public int Order { get; }
        public string ExternalKey { get; }
        public bool Active { get; }
        public Guid OrganisationClassificationTypeId { get; }
        public string OrganisationClassificationTypeName { get; }

        public OrganisationClassificationCreated(
            Guid organisationClassificationId,
            string name,
            int order,
            string externalKey,
            bool active,
            Guid organisationClassificationTypeId,
            string organisationClassificationTypeName)
        {
            Id = organisationClassificationId;

            Name = name;
            Order = order;
            ExternalKey = externalKey;
            Active = active;
            OrganisationClassificationTypeId = organisationClassificationTypeId;
            OrganisationClassificationTypeName = organisationClassificationTypeName;
        }
    }
}
