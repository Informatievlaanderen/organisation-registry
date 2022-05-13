namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using ContactType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyContactCommandHandler
    : BaseCommandHandler<UpdateBodyContactCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyContact>
{
    public UpdateBodyContactCommandHandler(ILogger<UpdateBodyContactCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyContact> envelope)
    {
        var contactType = Session.Get<ContactType>(envelope.Command.ContactTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateContact(
            envelope.Command.BodyContactId,
            contactType,
            envelope.Command.Value,
            new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));

        await Session.Commit(envelope.User);
    }
}
