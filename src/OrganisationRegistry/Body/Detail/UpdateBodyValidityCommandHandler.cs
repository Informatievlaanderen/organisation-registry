namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyValidityCommandHandler
    : BaseCommandHandler<UpdateBodyValidityCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyValidity>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateBodyValidityCommandHandler(
        ILogger<UpdateBodyValidityCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyValidity> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateFormalValidity(envelope.Command.FormalValidity);

        await Session.Commit(envelope.User);
    }
}
