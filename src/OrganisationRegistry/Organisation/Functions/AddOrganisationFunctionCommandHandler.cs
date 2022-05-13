namespace OrganisationRegistry.Organisation;

using System.Linq;
using System.Threading.Tasks;
using ContactType;
using Function;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Person;

public class AddOrganisationFunctionCommandHandler
:BaseCommandHandler<AddOrganisationFunctionCommandHandler>
,ICommandEnvelopeHandler<AddOrganisationFunction>
{
    public AddOrganisationFunctionCommandHandler(ILogger<AddOrganisationFunctionCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationFunction> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var person = session.Get<Person>(envelope.Command.PersonId);
                    var function = session.Get<FunctionType>(envelope.Command.FunctionTypeId);

                    var contacts = envelope.Command.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.AddFunction(
                        envelope.Command.OrganisationFunctionId,
                        function,
                        person,
                        contacts,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
