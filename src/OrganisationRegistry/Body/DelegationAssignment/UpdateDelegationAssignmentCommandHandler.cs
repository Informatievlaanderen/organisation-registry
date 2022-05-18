namespace OrganisationRegistry.Body;

using System.Linq;
using System.Threading.Tasks;
using ContactType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;
using Person;

public class UpdateDelegationAssignmentCommandHandler
:BaseCommandHandler<UpdateDelegationAssignmentCommandHandler>,
    ICommandEnvelopeHandler<UpdateDelegationAssignment>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateDelegationAssignmentCommandHandler(
        ILogger<UpdateDelegationAssignmentCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateDelegationAssignment> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var person = Session.Get<Person>(envelope.Command.PersonId);

        var contacts = envelope.Command.Contacts.Select(contact =>
        {
            var contactType = Session.Get<ContactType>(contact.Key);
            return new Contact(contactType, contact.Value);
        }).ToList();

        body.UpdatePersonAssignmentToDelegation(
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
