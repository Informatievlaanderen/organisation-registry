namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveDelegationAssignmentCommandHandler
    : BaseCommandHandler<RemoveDelegationAssignmentCommandHandler>,
        ICommandEnvelopeHandler<RemoveDelegationAssignment>
{
    public RemoveDelegationAssignmentCommandHandler(
        ILogger<RemoveDelegationAssignmentCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<RemoveDelegationAssignment> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.RemovePersonAssignmentFromDelegation(
                        envelope.Command.BodySeatId,
                        envelope.Command.BodyMandateId,
                        envelope.Command.DelegationAssignmentId);
                });
}
