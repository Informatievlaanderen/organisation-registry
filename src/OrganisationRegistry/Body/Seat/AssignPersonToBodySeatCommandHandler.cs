﻿namespace OrganisationRegistry.Body;

using System.Linq;
using System.Threading.Tasks;
using ContactType;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;
using Person;

public class AssignPersonToBodySeatCommandHandler
:BaseCommandHandler<AssignPersonToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<AssignPersonToBodySeat>
{
    public AssignPersonToBodySeatCommandHandler(ILogger<AssignPersonToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AssignPersonToBodySeat> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var person = session.Get<Person>(envelope.Command.PersonId);

                    var contacts = envelope.Command.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    body.AssignPersonToBodySeat(
                        person,
                        envelope.Command.BodyMandateId,
                        envelope.Command.BodySeatId,
                        contacts,
                        envelope.Command.Validity);
                });
}
