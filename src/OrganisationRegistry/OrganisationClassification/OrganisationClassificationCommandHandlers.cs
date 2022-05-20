namespace OrganisationRegistry.OrganisationClassification
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;
    using OrganisationClassificationType;

    public class OrganisationClassificationCommandHandlers :
        BaseCommandHandler<OrganisationClassificationCommandHandlers>,
        ICommandEnvelopeHandler<CreateOrganisationClassification>,
        ICommandEnvelopeHandler<UpdateOrganisationClassification>
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

        public async Task Handle(ICommandEnvelope<CreateOrganisationClassification> envelope)
        {
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(envelope.Command.OrganisationClassificationTypeId);

            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name, envelope.Command.OrganisationClassificationTypeId))
                throw new NameNotUniqueWithinType();

            if (_uniqueExternalKeyValidator.IsExternalKeyTaken(envelope.Command.ExternalKey, envelope.Command.OrganisationClassificationTypeId))
                throw new ExternalKeyNotUniqueWithinType();

            var organisationClassification =
                new OrganisationClassification(
                    envelope.Command.OrganisationClassificationId,
                    envelope.Command.Name,
                    envelope.Command.Order,
                    envelope.Command.ExternalKey,
                    envelope.Command.Active,
                    organisationClassificationType);

            Session.Add(organisationClassification);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateOrganisationClassification> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.OrganisationClassificationId, envelope.Command.Name, envelope.Command.OrganisationClassificationTypeId))
                throw new NameNotUniqueWithinType();

            if (_uniqueExternalKeyValidator.IsExternalKeyTaken(envelope.Command.OrganisationClassificationId, envelope.Command.ExternalKey,
                    envelope.Command.OrganisationClassificationTypeId))
                throw new ExternalKeyNotUniqueWithinType();

            var organisationClassificationType = Session.Get<OrganisationClassificationType>(envelope.Command.OrganisationClassificationTypeId);
            var organisationClassification = Session.Get<OrganisationClassification>(envelope.Command.OrganisationClassificationId);
            organisationClassification.Update(envelope.Command.Name, envelope.Command.Order, envelope.Command.ExternalKey, envelope.Command.Active, organisationClassificationType);
            await Session.Commit(envelope.User);
        }
    }
}
