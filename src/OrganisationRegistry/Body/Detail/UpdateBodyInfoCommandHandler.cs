namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyInfoCommandHandler
:BaseCommandHandler<UpdateBodyInfoCommandHandler>,
    ICommandEnvelopeHandler<UpdateBodyInfo>
{
    public UpdateBodyInfoCommandHandler(ILogger<UpdateBodyInfoCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyInfo> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateInfo(
                        envelope.Command.Name,
                        envelope.Command.ShortName,
                        envelope.Command.Description);
                });
}
