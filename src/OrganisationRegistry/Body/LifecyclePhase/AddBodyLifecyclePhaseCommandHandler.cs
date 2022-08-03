namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var lifecyclePhaseType = session.Get<LifecyclePhaseType>(envelope.Command.LifecyclePhaseTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.AddLifecyclePhase(
                        envelope.Command.BodyLifecyclePhaseId,
                        lifecyclePhaseType,
                        envelope.Command.Validity);
                });
}
