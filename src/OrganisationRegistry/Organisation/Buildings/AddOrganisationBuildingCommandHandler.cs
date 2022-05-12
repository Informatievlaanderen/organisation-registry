namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Building;
using Commands;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddOrganisationBuildingCommandHandler:
    BaseCommandHandler<AddOrganisationBuildingCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationBuilding>
{
    private readonly IDateTimeProvider _dateTimeProvider;


    public AddOrganisationBuildingCommandHandler(ILogger<AddOrganisationBuildingCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationBuilding> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var building = session.Get<Building>(envelope.Command.BuildingId);

                    organisation.AddBuilding(
                        envelope.Command.OrganisationBuildingId,
                        building,
                        envelope.Command.IsMainBuilding,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        _dateTimeProvider);
                });
}
