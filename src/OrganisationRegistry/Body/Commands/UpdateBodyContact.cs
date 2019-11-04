namespace OrganisationRegistry.Body.Commands
{
    using System;
    using ContactType;

    public class UpdateBodyContact : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public Guid BodyContactId { get; }
        public ContactTypeId ContactTypeId { get; }
        public string Value { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public UpdateBodyContact(
            Guid organisationContactId,
            BodyId organisationId,
            ContactTypeId contactTypeId,
            string value,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            BodyContactId = organisationContactId;
            ContactTypeId = contactTypeId;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
