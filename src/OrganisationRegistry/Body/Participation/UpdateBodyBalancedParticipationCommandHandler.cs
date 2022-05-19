namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyBalancedParticipationCommandHandler
:BaseCommandHandler<UpdateBodyBalancedParticipationCommandHandler>,
    ICommandEnvelopeHandler<UpdateBodyBalancedParticipation>
{
    public UpdateBodyBalancedParticipationCommandHandler(ILogger<UpdateBodyBalancedParticipationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyBalancedParticipation> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateBalancedParticipation(
            envelope.Command.BalancedParticipationObligatory,
            envelope.Command.BalancedParticipationExtraRemark,
            envelope.Command.BalancedParticipationExceptionMeasure);

        await Session.Commit(envelope.User);
    }
}
