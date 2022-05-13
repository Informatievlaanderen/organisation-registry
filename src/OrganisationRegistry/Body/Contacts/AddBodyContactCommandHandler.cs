namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using ContactType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddBodyContactCommandHandler
    : BaseCommandHandler<AddBodyContactCommandHandler>,
        ICommandEnvelopeHandler<AddBodyContact>
{
    public AddBodyContactCommandHandler(ILogger<AddBodyContactCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AddBodyContact> envelope)
    {
        var contactType = Session.Get<ContactType>(envelope.Command.ContactTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.AddContact(
            envelope.Command.BodyContactId,
            contactType,
            envelope.Command.ContactValue,
            new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));

        await Session.Commit(envelope.User);
    }
}
