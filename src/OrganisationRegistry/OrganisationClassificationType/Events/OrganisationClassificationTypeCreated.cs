namespace OrganisationRegistry.OrganisationClassificationType.Events
{
    using System;

    public class OrganisationClassificationTypeCreated : BaseEvent<OrganisationClassificationTypeCreated>
    {
        public Guid OrganisationClassificationTypeId => Id;

        public string Name { get; }

        public OrganisationClassificationTypeCreated(
            Guid organisationClassificationTypeId,
            string name)
        {
            Id = organisationClassificationTypeId;

            Name = name;
        }
    }
}
