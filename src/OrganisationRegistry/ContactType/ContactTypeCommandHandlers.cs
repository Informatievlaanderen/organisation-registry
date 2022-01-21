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
        ICommandHandler<CreateContactType>,
        ICommandHandler<UpdateContactType>
    {
        private readonly IUniqueNameValidator<ContactType> _uniqueNameValidator;

        public ContactTypeCommandHandlers(
            ILogger<ContactTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<ContactType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateContactType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var contactType = new ContactType(message.ContactTypeId, message.Name);
            Session.Add(contactType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateContactType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.ContactTypeId, message.Name))
                throw new NameNotUnique();

            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            contactType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
