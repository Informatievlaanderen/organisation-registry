namespace OrganisationRegistry.ContactType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
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
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var contactType = new ContactType(envelope.Command.ContactTypeId, envelope.Command.Name);
            Session.Add(contactType);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateContactType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.ContactTypeId, envelope.Command.Name))
                throw new NameNotUnique();

            var contactType = Session.Get<ContactType>(envelope.Command.ContactTypeId);
            contactType.Update(envelope.Command.Name);
            await Session.Commit(envelope.User);
        }
    }
}
