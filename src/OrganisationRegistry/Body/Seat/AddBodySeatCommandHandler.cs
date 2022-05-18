namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using SeatType;

public class AddBodySeatCommandHandler
    : BaseCommandHandler<AddBodySeatCommandHandler>,
        ICommandEnvelopeHandler<AddBodySeat>
{
    private readonly IBodySeatNumberGenerator _bodySeatNumberGenerator;

    public AddBodySeatCommandHandler(
        ILogger<AddBodySeatCommandHandler> logger,
        ISession session,
        IBodySeatNumberGenerator bodySeatNumberGenerator) : base(logger, session)
    {
        _bodySeatNumberGenerator = bodySeatNumberGenerator;
    }

    public async Task Handle(ICommandEnvelope<AddBodySeat> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var seatType = Session.Get<SeatType>(envelope.Command.SeatTypeId);
        var bodySeatNumber = _bodySeatNumberGenerator.GenerateNumber();

        body.AddSeat(
            envelope.Command.BodySeatId,
            envelope.Command.Name,
            bodySeatNumber,
            seatType,
            envelope.Command.PaidSeat,
            envelope.Command.EntitledToVote,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
