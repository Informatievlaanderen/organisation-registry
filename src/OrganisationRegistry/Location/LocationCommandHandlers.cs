namespace OrganisationRegistry.Location;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class LocationCommandHandlers :
    BaseCommandHandler<LocationCommandHandlers>,
    ICommandEnvelopeHandler<CreateLocation>,
    ICommandEnvelopeHandler<UpdateLocation>
{
    private readonly IUniqueNameValidator<Location> _uniqueNameValidator;

    public LocationCommandHandlers(
        ILogger<LocationCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<Location> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateLocation> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Address.FullAddress))
                        throw new NameNotUnique();

                    var location = new Location(envelope.Command.LocationId, envelope.Command.CrabLocationId, envelope.Command.Address);
                    session.Add(location);
                });

    public async Task Handle(ICommandEnvelope<UpdateLocation> envelope)
        => await UpdateHandler<Location>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.LocationId, envelope.Command.Address.FullAddress))
                        throw new NameNotUnique();

                    var location = session.Get<Location>(envelope.Command.LocationId);
                    location.Update(envelope.Command.CrabLocationId, envelope.Command.Address);
                });
}
