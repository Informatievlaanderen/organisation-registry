namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateMainBuildingCommandHandler : BaseCommandHandler<UpdateMainBuildingCommandHandler>,
    ICommandEnvelopeHandler<UpdateMainBuilding>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateMainBuildingCommandHandler(
        ILogger<UpdateMainBuildingCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateMainBuilding> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateMainBuilding(_dateTimeProvider.Today);
                });
}
