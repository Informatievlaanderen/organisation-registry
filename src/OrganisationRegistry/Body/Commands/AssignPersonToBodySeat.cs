namespace OrganisationRegistry.Body.Commands
{
    using Person;
    using System.Collections.Generic;
    using ContactType;

    public class AssignPersonToBodySeat : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }
        public BodyMandateId BodyMandateId { get; }
        public PersonId PersonId { get; }
        public Dictionary<ContactTypeId, string> Contacts { get; }
        public Period Validity { get; }

        public AssignPersonToBodySeat(
            BodyId bodyId,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            PersonId personId,
            Dictionary<ContactTypeId, string> contacts,
            Period validity)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            BodyMandateId = bodyMandateId;
            PersonId = personId;
            Contacts = contacts ?? new Dictionary<ContactTypeId, string>();
            Validity = validity;
        }
    }
}
