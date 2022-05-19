namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
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
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.RemovePersonAssignmentFromDelegation(
            envelope.Command.BodySeatId,
            envelope.Command.BodyMandateId,
            envelope.Command.DelegationAssignmentId);

        await Session.Commit(envelope.User);
    }
}
