namespace OrganisationRegistry.LocationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class LocationTypeCommandHandlers :
    BaseCommandHandler<LocationTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateLocationType>,
    ICommandEnvelopeHandler<UpdateLocationType>
{
    private readonly IUniqueNameValidator<LocationType> _uniqueNameValidator;

    public LocationTypeCommandHandlers(
        ILogger<LocationTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<LocationType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateLocationType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var locationType = new LocationType(envelope.Command.LocationTypeId, envelope.Command.Name);
                    session.Add(locationType);
                });

    public async Task Handle(ICommandEnvelope<UpdateLocationType> envelope)
        => await UpdateHandler<LocationType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.LocationTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var locationType = session.Get<LocationType>(envelope.Command.LocationTypeId);
                    locationType.Update(envelope.Command.Name);
                });
}
