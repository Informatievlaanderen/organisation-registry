namespace OrganisationRegistry.LocationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
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
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var locationType = new LocationType(envelope.Command.LocationTypeId, envelope.Command.Name);
        Session.Add(locationType);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateLocationType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.LocationTypeId, envelope.Command.Name))
            throw new NameNotUnique();

        var locationType = Session.Get<LocationType>(envelope.Command.LocationTypeId);
        locationType.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}
