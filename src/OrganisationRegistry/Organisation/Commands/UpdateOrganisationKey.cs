namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using KeyTypes;

    public class UpdateOrganisationKey : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationKeyId { get; }
        public KeyTypeId KeyTypeId { get; }
        public string Value { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public UpdateOrganisationKey(
            Guid organisationKeyId,
            OrganisationId organisationId,
            KeyTypeId keyTypeId,
            string value,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationKeyId = organisationKeyId;
            KeyTypeId = keyTypeId;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
