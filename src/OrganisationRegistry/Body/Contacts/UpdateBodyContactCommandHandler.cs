namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using ContactType;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var contactType = session.Get<ContactType>(envelope.Command.ContactTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateContact(
                        envelope.Command.BodyContactId,
                        contactType,
                        envelope.Command.Value,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
