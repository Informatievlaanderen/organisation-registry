namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateFormalValidity(envelope.Command.FormalValidity);
                });
}
