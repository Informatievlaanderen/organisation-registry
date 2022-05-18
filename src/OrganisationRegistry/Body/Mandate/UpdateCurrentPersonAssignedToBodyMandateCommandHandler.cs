namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateCurrentPersonAssignedToBodyMandateCommandHandler
:BaseCommandHandler<UpdateCurrentPersonAssignedToBodyMandateCommandHandler>,
    ICommandEnvelopeHandler<UpdateCurrentPersonAssignedToBodyMandate>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateCurrentPersonAssignedToBodyMandateCommandHandler(
        ILogger<UpdateCurrentPersonAssignedToBodyMandateCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateCurrentPersonAssignedToBodyMandate> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        foreach (var (bodySeatId, bodyMandateId) in envelope.Command.MandatesToUpdate)
        {
            body.UpdateCurrentPersonAssignedToBodyMandate(
                bodySeatId,
                bodyMandateId,
                _dateTimeProvider.Today);
        }

        await Session.Commit(envelope.User);
    }
}
