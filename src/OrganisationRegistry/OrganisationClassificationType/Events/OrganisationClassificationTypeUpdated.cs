namespace OrganisationRegistry.OrganisationClassificationType.Events
{
    using System;
    using Newtonsoft.Json;

    public class OrganisationClassificationTypeUpdated : BaseEvent<OrganisationClassificationTypeUpdated>
    {
        public Guid OrganisationClassificationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public OrganisationClassificationTypeUpdated(
            OrganisationClassificationTypeId organisationClassificationTypeId,
            OrganisationClassificationTypeName name,
            OrganisationClassificationTypeName previousName)
        {
            Id = organisationClassificationTypeId;

            Name = name;
            PreviousName = previousName;
        }

        [JsonConstructor]
        public OrganisationClassificationTypeUpdated(
            Guid organisationClassificationTypeId,
            string name,
            string previousName)
            : this(
                new OrganisationClassificationTypeId(organisationClassificationTypeId),
                new OrganisationClassificationTypeName(name),
                new OrganisationClassificationTypeName(previousName)) { }
    }
}
