namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class UpdateBodyOrganisationCommandHandler
    : BaseCommandHandler<UpdateBodyOrganisationCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyOrganisation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateBodyOrganisationCommandHandler(
        ILogger<UpdateBodyOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyOrganisation> envelope)
    {
        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateOrganisation(
            envelope.Command.BodyOrganisationId,
            organisation,
            envelope.Command.Validity,
            _dateTimeProvider);

        await Session.Commit(envelope.User);
    }
}
