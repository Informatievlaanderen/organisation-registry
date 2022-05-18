namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateCurrentBodyOrganisationCommandHandler
    : BaseCommandHandler<UpdateCurrentBodyOrganisationCommandHandler>,
        ICommandEnvelopeHandler<UpdateCurrentBodyOrganisation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateCurrentBodyOrganisationCommandHandler(
        ILogger<UpdateCurrentBodyOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateCurrentBodyOrganisation> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        body.UpdateCurrentOrganisation(_dateTimeProvider.Today);

        await Session.Commit(envelope.User);
    }
}
