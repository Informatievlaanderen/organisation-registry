namespace OrganisationRegistry.OrganisationClassification
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;
    using OrganisationClassificationType;

    public class OrganisationClassificationCommandHandlers :
        BaseCommandHandler<OrganisationClassificationCommandHandlers>,
        ICommandHandler<CreateOrganisationClassification>,
        ICommandHandler<UpdateOrganisationClassification>
    {
        private readonly IUniqueNameWithinTypeValidator<OrganisationClassification> _uniqueNameValidator;
        private readonly IUniqueExternalKeyWithinTypeValidator<OrganisationClassification> _uniqueExternalKeyValidator;

        public OrganisationClassificationCommandHandlers(
            ILogger<OrganisationClassificationCommandHandlers> logger,
            ISession session,
            IUniqueNameWithinTypeValidator<OrganisationClassification> uniqueNameValidator,
            IUniqueExternalKeyWithinTypeValidator<OrganisationClassification> uniqueExternalKeyValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
            _uniqueExternalKeyValidator = uniqueExternalKeyValidator;
        }

        public void Handle(CreateOrganisationClassification message)
        {
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);

            if (_uniqueNameValidator.IsNameTaken(message.Name, message.OrganisationClassificationTypeId))
                throw new NameNotUniqueWithinTypeException();

            if (_uniqueExternalKeyValidator.IsExternalKeyTaken(message.ExternalKey, message.OrganisationClassificationTypeId))
                throw new ExternalKeyNotUniqueWithinTypeException();

            var organisationClassification =
                new OrganisationClassification(
                    message.OrganisationClassificationId,
                    message.Name,
                    message.Order,
                    message.ExternalKey,
                    message.Active,
                    organisationClassificationType);

            Session.Add(organisationClassification);
            Session.Commit();
        }

        public void Handle(UpdateOrganisationClassification message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.OrganisationClassificationId, message.Name, message.OrganisationClassificationTypeId))
                throw new NameNotUniqueWithinTypeException();

            if (_uniqueExternalKeyValidator.IsExternalKeyTaken(message.OrganisationClassificationId, message.ExternalKey,
                message.OrganisationClassificationTypeId))
                throw new ExternalKeyNotUniqueWithinTypeException();

            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);
            var organisationClassification = Session.Get<OrganisationClassification>(message.OrganisationClassificationId);
            organisationClassification.Update(message.Name, message.Order, message.ExternalKey, message.Active, organisationClassificationType);
            Session.Commit();
        }
    }
}
