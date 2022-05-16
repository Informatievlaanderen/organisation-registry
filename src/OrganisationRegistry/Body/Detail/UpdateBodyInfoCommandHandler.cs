namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
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
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateInfo(
            envelope.Command.Name,
            envelope.Command.ShortName,
            envelope.Command.Description);

        await Session.Commit(envelope.User);
    }
}
