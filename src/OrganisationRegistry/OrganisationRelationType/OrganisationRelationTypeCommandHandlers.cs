namespace OrganisationRegistry.OrganisationRelationType
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class OrganisationRelationTypeCommandHandlers :
        BaseCommandHandler<OrganisationRelationTypeCommandHandlers>,
        ICommandHandler<CreateOrganisationRelationType>,
        ICommandHandler<UpdateOrganisationRelationType>
    {
        private readonly IUniqueNameValidator<OrganisationRelationType> _uniqueNameValidator;

        public OrganisationRelationTypeCommandHandlers(
            ILogger<OrganisationRelationTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<OrganisationRelationType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateOrganisationRelationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var organisationRelationType = new OrganisationRelationType(message.OrganisationRelationTypeId, message.Name, message.InverseName);
            Session.Add(organisationRelationType);
            Session.Commit();
        }

        public void Handle(UpdateOrganisationRelationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.OrganisationRelationTypeId, message.Name))
                throw new NameNotUniqueException();

            var organisationRelationType = Session.Get<OrganisationRelationType>(message.OrganisationRelationTypeId);
            organisationRelationType.Update(message.Name, message.InverseName);
            Session.Commit();
        }
    }
}
