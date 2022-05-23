namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using Infrastructure.Domain;
using ContactType;
using Handling;

public class UpdateOrganisationContactCommandHandler
    : BaseCommandHandler<UpdateOrganisationContactCommandHandler>,
        ICommandEnvelopeHandler<UpdateOrganisationContact>
{
    public UpdateOrganisationContactCommandHandler(ILogger<UpdateOrganisationContactCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationContact> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var contactType = session.Get<ContactType>(envelope.Command.ContactTypeId);

                    organisation.UpdateContact(
                        envelope.Command.OrganisationContactId,
                        contactType,
                        envelope.Command.Value,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
