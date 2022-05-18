namespace OrganisationRegistry.Body;

using System.Collections.Generic;
using ContactType;
using Person;

public class ReassignPersonToBodySeat : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodySeatId BodySeatId { get; }
    public BodyMandateId BodyMandateId { get; }
    public PersonId PersonId { get; }
    public Dictionary<ContactTypeId, string> Contacts { get; }
    public Period Validity { get; }

    public ReassignPersonToBodySeat(
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
        Contacts = contacts ?? new Dictionary<ContactTypeId, string>();
        PersonId = personId;
        Validity = validity;
    }
}
