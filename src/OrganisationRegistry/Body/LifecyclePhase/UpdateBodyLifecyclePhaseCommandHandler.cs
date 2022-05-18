namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
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
    {
        var lifecyclePhaseType = Session.Get<LifecyclePhaseType>(envelope.Command.LifecyclePhaseTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateLifecyclePhase(
            envelope.Command.BodyLifecyclePhaseId,
            lifecyclePhaseType,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
