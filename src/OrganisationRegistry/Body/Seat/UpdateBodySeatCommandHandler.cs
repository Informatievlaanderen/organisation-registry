namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using SeatType;

public class UpdateBodySeatCommandHandler
    : BaseCommandHandler<UpdateBodySeatCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodySeat>
{
    public UpdateBodySeatCommandHandler(ILogger<UpdateBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodySeat> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var seatType = Session.Get<SeatType>(envelope.Command.SeatTypeId);

        body.UpdateSeat(
            envelope.Command.BodySeatId,
            envelope.Command.Name,
            seatType,
            envelope.Command.PaidSeat,
            envelope.Command.EntitledToVote,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
