namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateCurrentOrganisationParentCommandHandler
    : BaseCommandHandler<UpdateCurrentOrganisationParentCommandHandler>,
        ICommandEnvelopeHandler<UpdateCurrentOrganisationParent>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateCurrentOrganisationParentCommandHandler(
        ILogger<UpdateCurrentOrganisationParentCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateCurrentOrganisationParent> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateCurrentOrganisationParent(_dateTimeProvider.Today);
                });
}
