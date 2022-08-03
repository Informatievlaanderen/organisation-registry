namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var seatType = session.Get<SeatType>(envelope.Command.SeatTypeId);
                    var bodySeatNumber = _bodySeatNumberGenerator.GenerateNumber();

                    body.AddSeat(
                        envelope.Command.BodySeatId,
                        envelope.Command.Name,
                        bodySeatNumber,
                        seatType,
                        envelope.Command.PaidSeat,
                        envelope.Command.EntitledToVote,
                        envelope.Command.Validity);
                });
}
