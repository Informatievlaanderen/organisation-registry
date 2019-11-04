namespace OrganisationRegistry.OrganisationClassificationType
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class OrganisationClassificationTypeCommandHandlers :
        BaseCommandHandler<OrganisationClassificationTypeCommandHandlers>,
        ICommandHandler<CreateOrganisationClassificationType>,
        ICommandHandler<UpdateOrganisationClassificationType>
    {
        private readonly IUniqueNameValidator<OrganisationClassificationType> _uniqueNameValidator;

        public OrganisationClassificationTypeCommandHandlers(
            ILogger<OrganisationClassificationTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<OrganisationClassificationType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateOrganisationClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var organisationClassificationType = new OrganisationClassificationType(message.OrganisationClassificationTypeId, message.Name);
            Session.Add(organisationClassificationType);
            Session.Commit();
        }

        public void Handle(UpdateOrganisationClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.OrganisationClassificationTypeId, message.Name))
                throw new NameNotUniqueException();

            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);
            organisationClassificationType.Update(message.Name);
            Session.Commit();
        }
    }
}
