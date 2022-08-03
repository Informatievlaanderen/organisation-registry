namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using LifecyclePhaseType;
using Microsoft.Extensions.Logging;

public class UpdateBodyLifecyclePhaseCommandHandler
: BaseCommandHandler<UpdateBodyLifecyclePhaseCommandHandler>,
    ICommandEnvelopeHandler<UpdateBodyLifecyclePhase>
{

    public UpdateBodyLifecyclePhaseCommandHandler(ILogger<UpdateBodyLifecyclePhaseCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyLifecyclePhase> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var lifecyclePhaseType = session.Get<LifecyclePhaseType>(envelope.Command.LifecyclePhaseTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateLifecyclePhase(
                        envelope.Command.BodyLifecyclePhaseId,
                        lifecyclePhaseType,
                        envelope.Command.Validity);
                });
}
