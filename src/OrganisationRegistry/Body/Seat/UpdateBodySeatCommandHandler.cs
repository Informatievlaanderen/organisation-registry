namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var seatType = session.Get<SeatType>(envelope.Command.SeatTypeId);

                    body.UpdateSeat(
                        envelope.Command.BodySeatId,
                        envelope.Command.Name,
                        seatType,
                        envelope.Command.PaidSeat,
                        envelope.Command.EntitledToVote,
                        envelope.Command.Validity);
                });
}
