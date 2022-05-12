namespace OrganisationRegistry.Organisation.Locations;

using System.Threading.Tasks;
using Commands;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Location;
using LocationType;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationLocationCommandHandler:
    BaseCommandHandler<UpdateOrganisationLocationCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationLocation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationLocationCommandHandler(ILogger<UpdateOrganisationLocationCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationLocation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User ,Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var location = session.Get<Location>(envelope.Command.LocationId);
                    var locationType = envelope.Command.LocationTypeId != null
                        ? session.Get<LocationType>(envelope.Command.LocationTypeId)
                        : null;

                    organisation.UpdateLocation(
                        envelope.Command.OrganisationLocationId,
                        location,
                        envelope.Command.IsMainLocation,
                        locationType,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        envelope.Command.Source,
                        _dateTimeProvider);
                });
}
