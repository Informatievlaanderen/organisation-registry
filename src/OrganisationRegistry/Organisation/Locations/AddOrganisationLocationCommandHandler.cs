namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Location;
using LocationType;
using Microsoft.Extensions.Logging;

public class AddOrganisationLocationCommandHandler:
    BaseCommandHandler<AddOrganisationLocationCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationLocation>
{

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    public AddOrganisationLocationCommandHandler(ILogger<AddOrganisationLocationCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider, IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationLocation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var location = session.Get<Location>(envelope.Command.LocationId);
                    var locationType = GetLocationType(session, envelope.Command.LocationTypeId);

                    KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

                    organisation.AddLocation(
                        envelope.Command.OrganisationLocationId,
                        location,
                        envelope.Command.IsMainLocation,
                        locationType,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        Source.Wegwijs,
                        _dateTimeProvider);
                });

    private static LocationType? GetLocationType(ISession session, LocationTypeId? maybeLocationTypeId)
        => maybeLocationTypeId is { } locationTypeId
            ? session.Get<LocationType>(locationTypeId)
            : null;
}
