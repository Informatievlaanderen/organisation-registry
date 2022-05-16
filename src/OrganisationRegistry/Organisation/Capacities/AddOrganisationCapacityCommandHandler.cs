namespace OrganisationRegistry.Organisation;

using System.Linq;
using System.Threading.Tasks;
using Capacity;
using ContactType;
using Function;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Location;
using Microsoft.Extensions.Logging;
using Person;

public class AddOrganisationCapacityCommandHandler
    : BaseCommandHandler<AddOrganisationCapacityCommandHandler>
        , ICommandEnvelopeHandler<AddOrganisationCapacity>

{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddOrganisationCapacityCommandHandler(
        ILogger<AddOrganisationCapacityCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationCapacity> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithCapacityPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var capacity = session.Get<Capacity>(envelope.Command.CapacityId);
                    var person = envelope.Command.PersonId is { }
                        ? session.Get<Person>(envelope.Command.PersonId)
                        : null;
                    var function = envelope.Command.FunctionId is { }
                        ? session.Get<FunctionType>(envelope.Command.FunctionId)
                        : null;
                    var location = envelope.Command.LocationId is { }
                        ? session.Get<Location>(envelope.Command.LocationId)
                        : null;

                    var contacts = envelope.Command.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.AddCapacity(
                        envelope.Command.OrganisationCapacityId,
                        capacity,
                        person,
                        function,
                        location,
                        contacts,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        _dateTimeProvider);
                });
}
