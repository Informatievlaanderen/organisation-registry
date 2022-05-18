namespace OrganisationRegistry.Body;

using System.Linq;
using System.Threading.Tasks;
using ContactType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;
using Person;

public class ReassignPersonToBodySeatCommandHandler
:BaseCommandHandler<ReassignPersonToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<ReassignPersonToBodySeat>
{
    public ReassignPersonToBodySeatCommandHandler(ILogger<ReassignPersonToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<ReassignPersonToBodySeat> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var person = Session.Get<Person>(envelope.Command.PersonId);

        var contacts = envelope.Command.Contacts.Select(contact =>
        {
            var contactType = Session.Get<ContactType>(contact.Key);
            return new Contact(contactType, contact.Value);
        }).ToList();

        body.ReassignPersonToBodySeat(
            person,
            envelope.Command.BodyMandateId,
            envelope.Command.BodySeatId,
            contacts,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
