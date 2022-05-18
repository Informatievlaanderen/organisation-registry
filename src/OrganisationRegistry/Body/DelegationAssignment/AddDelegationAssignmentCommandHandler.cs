namespace OrganisationRegistry.Body;

using System.Linq;
using System.Threading.Tasks;
using ContactType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;
using Person;

public class AddDelegationAssignmentCommandHandler
:BaseCommandHandler<AddDelegationAssignmentCommandHandler>,
    ICommandEnvelopeHandler<AddDelegationAssignment>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddDelegationAssignmentCommandHandler(
        ILogger<AddDelegationAssignmentCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<AddDelegationAssignment> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var person = Session.Get<Person>(envelope.Command.PersonId);

        var contacts = envelope.Command.Contacts.Select(contact =>
        {
            var contactType = Session.Get<ContactType>(contact.Key);
            return new Contact(contactType, contact.Value);
        }).ToList();

        body.AssignPersonToDelegation(
            envelope.Command.BodySeatId,
            envelope.Command.BodyMandateId,
            envelope.Command.DelegationAssignmentId,
            person,
            contacts,
            envelope.Command.Validity,
            _dateTimeProvider.Today);

        await Session.Commit(envelope.User);
    }
}
