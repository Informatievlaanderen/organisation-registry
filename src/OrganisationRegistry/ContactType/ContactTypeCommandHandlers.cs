namespace OrganisationRegistry.ContactType
{
    using Commands;
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

        public void Handle(CreateContactType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var contactType = new ContactType(message.ContactTypeId, message.Name);
            Session.Add(contactType);
            Session.Commit();
        }

        public void Handle(UpdateContactType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.ContactTypeId, message.Name))
                throw new NameNotUniqueException();

            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            contactType.Update(message.Name);
            Session.Commit();
        }
    }
}
