namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Building;
using Commands;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationBuildingCommandHandler :
    BaseCommandHandler<UpdateOrganisationBuildingCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationBuilding>
{

    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationBuildingCommandHandler(ILogger<UpdateOrganisationBuildingCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationBuilding> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var building = session.Get<Building>(envelope.Command.BuildingId);

                    organisation.UpdateBuilding(
                        envelope.Command.OrganisationBuildingId,
                        building,
                        envelope.Command.IsMainBuilding,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        _dateTimeProvider);
                });
}
