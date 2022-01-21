namespace OrganisationRegistry.OrganisationClassificationType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
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

        public async Task Handle(CreateOrganisationClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var organisationClassificationType = new OrganisationClassificationType(message.OrganisationClassificationTypeId, message.Name);
            Session.Add(organisationClassificationType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.OrganisationClassificationTypeId, message.Name))
                throw new NameNotUnique();

            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);
            organisationClassificationType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
