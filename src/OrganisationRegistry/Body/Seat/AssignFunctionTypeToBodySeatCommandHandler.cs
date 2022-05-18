namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Function;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class AssignFunctionTypeToBodySeatCommandHandler
:BaseCommandHandler<AssignFunctionTypeToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<AssignFunctionTypeToBodySeat>
{
    public AssignFunctionTypeToBodySeatCommandHandler(ILogger<AssignFunctionTypeToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AssignFunctionTypeToBodySeat> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);
        var functionType = Session.Get<FunctionType>(envelope.Command.FunctionTypeId);

        body.AssignFunctionTypeToBodySeat(
            organisation,
            functionType,
            envelope.Command.BodyMandateId,
            envelope.Command.BodySeatId,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
