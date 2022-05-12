namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
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
                    var locationType = GetLocationType(session, envelope.Command.LocationTypeId);

                    organisation.UpdateLocation(
                        envelope.Command.OrganisationLocationId,
                        location,
                        envelope.Command.IsMainLocation,
                        locationType,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        envelope.Command.Source,
                        _dateTimeProvider);
                });

    private static LocationType? GetLocationType(ISession session, LocationTypeId? maybeLocationTypeId)
        => maybeLocationTypeId is { } locationTypeId
            ? session.Get<LocationType>(locationTypeId)
            : null;
}
