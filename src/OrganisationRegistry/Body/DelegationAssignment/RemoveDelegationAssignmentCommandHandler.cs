namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveDelegationAssignmentCommandHandler
:BaseCommandHandler<RemoveDelegationAssignmentCommandHandler>,
    ICommandEnvelopeHandler<RemoveDelegationAssignment>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public RemoveDelegationAssignmentCommandHandler(
        ILogger<RemoveDelegationAssignmentCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<RemoveDelegationAssignment> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.RemovePersonAssignmentFromDelegation(
            envelope.Command.BodySeatId,
            envelope.Command.BodyMandateId,
            envelope.Command.DelegationAssignmentId,
            _dateTimeProvider.Today);

        await Session.Commit(envelope.User);
    }
}
