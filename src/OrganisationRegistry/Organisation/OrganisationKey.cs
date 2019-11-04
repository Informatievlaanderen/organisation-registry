namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationKey
    {
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public Guid OrganisationKeyId { get; }
        public Guid KeyTypeId { get; }
        public string KeyTypeName { get; }
        public string Value { get; }
        public Period Validity { get; }

        public OrganisationKey(
            Guid organisationKeyId,
            Guid organisationId,
            Guid keyTypeId,
            string keyTypeName,
            string value,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationKeyId = organisationKeyId;
            KeyTypeId = keyTypeId;
            Value = value;
            Validity = validity;
            KeyTypeName = keyTypeName;
        }
    }
}
