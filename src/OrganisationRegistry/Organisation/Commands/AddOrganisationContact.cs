namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using ContactType;

    public class AddOrganisationContact : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationContactId { get; }
        public ContactTypeId ContactTypeId { get; }
        public string ContactValue { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationContact(
            Guid organisationContactId,
            OrganisationId organisationId,
            ContactTypeId contactTypeId,
            string contactValue,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationContactId = organisationContactId;
            ContactTypeId = contactTypeId;
            ContactValue = contactValue;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
