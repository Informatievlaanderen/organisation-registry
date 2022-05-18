namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AssignBodySeatCommandHandler
:BaseCommandHandler<AssignBodySeatCommandHandler>,
    ICommandEnvelopeHandler<AssignBodySeatNumber>
{
    private readonly IBodySeatNumberGenerator _bodySeatNumberGenerator;

    public AssignBodySeatCommandHandler(
        ILogger<AssignBodySeatCommandHandler> logger,
        ISession session,
        IBodySeatNumberGenerator bodySeatNumberGenerator) : base(logger, session)
    {
        _bodySeatNumberGenerator = bodySeatNumberGenerator;
    }

    public async Task Handle(ICommandEnvelope<AssignBodySeatNumber> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var bodySeatNumber = _bodySeatNumberGenerator.GenerateNumber();

        body.AssignBodySeatNumberToBodySeat(
            envelope.Command.BodySeatId,
            bodySeatNumber);

        await Session.Commit(envelope.User);
    }
}
