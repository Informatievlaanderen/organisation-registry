namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using ContactType;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var contactType = session.Get<ContactType>(envelope.Command.ContactTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.AddContact(
                        envelope.Command.BodyContactId,
                        contactType,
                        envelope.Command.ContactValue,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
