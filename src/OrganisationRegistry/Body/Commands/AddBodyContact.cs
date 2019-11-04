namespace OrganisationRegistry.Body.Commands
{
    using System;
    using ContactType;

    public class AddBodyContact : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public Guid BodyContactId { get; }
        public ContactTypeId ContactTypeId { get; }
        public string ContactValue { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddBodyContact(
            Guid organisationContactId,
            BodyId organisationId,
            ContactTypeId contactTypeId,
            string contactValue,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            BodyContactId = organisationContactId;
            ContactTypeId = contactTypeId;
            ContactValue = contactValue;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
