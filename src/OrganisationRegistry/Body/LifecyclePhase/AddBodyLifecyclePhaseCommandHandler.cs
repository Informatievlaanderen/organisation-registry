namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using LifecyclePhaseType;
using Microsoft.Extensions.Logging;

public class AddBodyLifecyclePhaseCommandHandler
:BaseCommandHandler<AddBodyLifecyclePhaseCommandHandler>,
    ICommandEnvelopeHandler<AddBodyLifecyclePhase>
{
    public AddBodyLifecyclePhaseCommandHandler(ILogger<AddBodyLifecyclePhaseCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AddBodyLifecyclePhase> envelope)
    {
        var lifecyclePhaseType = Session.Get<LifecyclePhaseType>(envelope.Command.LifecyclePhaseTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.AddLifecyclePhase(
            envelope.Command.BodyLifecyclePhaseId,
            lifecyclePhaseType,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
