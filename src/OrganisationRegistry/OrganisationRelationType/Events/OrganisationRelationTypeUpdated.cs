namespace OrganisationRegistry.OrganisationRelationType.Events
{
    using System;

    public class OrganisationRelationTypeUpdated : BaseEvent<OrganisationRelationTypeUpdated>
    {
        public Guid OrganisationRelationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public string InverseName { get; set; }
        public string PreviousInverseName { get; set; }

        public OrganisationRelationTypeUpdated(
            Guid organisationRelationTypeId,
            string name,
            string? inverseName,
            string previousName,
            string? previousInverseName)
        {
            Id = organisationRelationTypeId;

            Name = name;
            InverseName = inverseName ?? string.Empty;

            PreviousName = previousName;
            PreviousInverseName = previousInverseName ?? string.Empty;
        }
    }
}
