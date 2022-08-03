namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using ContactType;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddOrganisationContactCommandHandler :
    BaseCommandHandler<AddOrganisationContactCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationContact>
{
    public AddOrganisationContactCommandHandler(
        ILogger<AddOrganisationContactCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationContact> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithBeheerderForOrganisationPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var contactType = session.Get<ContactType>(envelope.Command.ContactTypeId);

                    organisation.AddContact(
                        envelope.Command.OrganisationContactId,
                        contactType,
                        envelope.Command.ContactValue,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
