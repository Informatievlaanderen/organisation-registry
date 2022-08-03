namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Function;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class ReassignFunctionTypeToBodySeatCommandHandler
:BaseCommandHandler<ReassignFunctionTypeToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<ReassignFunctionTypeToBodySeat>
{
    public ReassignFunctionTypeToBodySeatCommandHandler(ILogger<ReassignFunctionTypeToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<ReassignFunctionTypeToBodySeat> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    var functionType = session.Get<FunctionType>(envelope.Command.FunctionTypeId);

                    body.ReassignFunctionTypeToBodySeat(
                        organisation,
                        functionType,
                        envelope.Command.BodyMandateId,
                        envelope.Command.BodySeatId,
                        envelope.Command.Validity);
                });
}
