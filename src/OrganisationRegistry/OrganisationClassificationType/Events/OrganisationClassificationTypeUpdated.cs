namespace OrganisationRegistry.OrganisationClassificationType.Events
{
    using System;

    public class OrganisationClassificationTypeUpdated : BaseEvent<OrganisationClassificationTypeUpdated>
    {
        public Guid OrganisationClassificationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public OrganisationClassificationTypeUpdated(
            Guid organisationClassificationTypeId,
            string name,
            string previousName)
        {
            Id = organisationClassificationTypeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
