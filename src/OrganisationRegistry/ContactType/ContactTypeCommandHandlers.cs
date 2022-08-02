namespace OrganisationRegistry.ContactType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class ContactTypeCommandHandlers :
    BaseCommandHandler<ContactTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateContactType>,
    ICommandEnvelopeHandler<UpdateContactType>
{
    private readonly IUniqueNameValidator<ContactType> _uniqueNameValidator;

    public ContactTypeCommandHandlers(
        ILogger<ContactTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<ContactType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateContactType> envelope)
    {
        await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var contactType = new ContactType(
                        envelope.Command.ContactTypeId,
                        envelope.Command.Name,
                        envelope.Command.Regex,
                        envelope.Command.Example);
                    session.Add(contactType);
                });
    }

    public async Task Handle(ICommandEnvelope<UpdateContactType> envelope)
    {
        await UpdateHandler<ContactType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.ContactTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var contactType = session.Get<ContactType>(envelope.Command.ContactTypeId);
                    contactType.Update(envelope.Command.Name, envelope.Command.Regex, envelope.Command.Example);
                });
    }
}
