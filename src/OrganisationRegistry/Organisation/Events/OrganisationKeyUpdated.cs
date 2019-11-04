namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationKeyUpdated : BaseEvent<OrganisationKeyUpdated>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationKeyId { get; }

        public Guid KeyTypeId { get; }
        public Guid PreviousKeyTypeId { get; }

        public string KeyTypeName { get; }
        public string PreviousKeyTypeName { get; }

        public string Value { get; }
        public string PreviousValue { get; }

        public DateTime? ValidFrom { get; }
        public DateTime? PreviouslyValidFrom { get; }

        public DateTime? ValidTo { get; }
        public DateTime? PreviouslyValidTo { get; }

        public OrganisationKeyUpdated(
            Guid organisationId,
            Guid organisationKeyId,
            Guid keyTypeId,
            string keyTypeName,
            string value,
            DateTime? validFrom,
            DateTime? validTo,
            Guid previousKeyTypeId,
            string previousKeyTypeName,
            string previousValue,
            DateTime? previouslyValidFrom,
            DateTime? previouslyValidTo)
        {
            Id = organisationId;

            OrganisationKeyId = organisationKeyId;
            KeyTypeId = keyTypeId;
            KeyTypeName = keyTypeName;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;

            PreviousKeyTypeId = previousKeyTypeId;
            PreviousKeyTypeName = previousKeyTypeName;
            PreviousValue = previousValue;
            PreviouslyValidFrom = previouslyValidFrom;
            PreviouslyValidTo = previouslyValidTo;
        }
    }
}
