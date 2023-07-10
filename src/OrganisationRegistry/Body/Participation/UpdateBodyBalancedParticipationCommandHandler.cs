namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.OrgaanBeheerder)
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    body.UpdateBalancedParticipation(
                        envelope.Command.BalancedParticipationObligatory,
                        envelope.Command.BalancedParticipationExtraRemark,
                        envelope.Command.BalancedParticipationExceptionMeasure);
                });
}
