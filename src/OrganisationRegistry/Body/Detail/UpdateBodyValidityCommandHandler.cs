namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyValidityCommandHandler
    : BaseCommandHandler<UpdateBodyValidityCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyValidity>
{
    public UpdateBodyValidityCommandHandler(
        ILogger<UpdateBodyValidityCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyValidity> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateFormalValidity(envelope.Command.FormalValidity);

        await Session.Commit(envelope.User);
    }
}
