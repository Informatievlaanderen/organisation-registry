namespace OrganisationRegistry.Organisation;

using System.Linq;
using System.Threading.Tasks;
using Capacity;
using ContactType;
using Function;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Location;
using Microsoft.Extensions.Logging;
using Person;

public class UpdateOrganisationCapacityCommandHandler
:BaseCommandHandler<UpdateOrganisationCapacityCommandHandler>
,ICommandEnvelopeHandler<UpdateOrganisationCapacity>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationCapacityCommandHandler(ILogger<UpdateOrganisationCapacityCommandHandler> logger, ISession session, IOrganisationRegistryConfiguration organisationRegistryConfiguration, IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationCapacity> envelope)
        => Handle(envelope.Command, envelope.User);

    public Task Handle(UpdateOrganisationCapacity message, IUser envelopeUser)
        => UpdateHandler<Organisation>.For(message,envelopeUser, Session)
            .WithCapacityPolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(envelopeUser);

                    var capacity = session.Get<Capacity>(message.CapacityId);
                    var person = message.PersonId is { } ? session.Get<Person>(message.PersonId) : null;
                    var function = message.FunctionTypeId is { }
                        ? session.Get<FunctionType>(message.FunctionTypeId)
                        : null;
                    var location = message.LocationId is { } ? session.Get<Location>(message.LocationId) : null;

                    var contacts = message.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.UpdateCapacity(
                        message.OrganisationCapacityId,
                        capacity,
                        person,
                        function,
                        location,
                        contacts,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        _dateTimeProvider);
                });
}
