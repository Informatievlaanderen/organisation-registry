namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using OrganisationClassification;
using OrganisationClassificationType;

public class UpdateOrganisationOrganisationClassificationCommandHandler
    :BaseCommandHandler<UpdateOrganisationOrganisationClassificationCommandHandler>
,ICommandEnvelopeHandler<UpdateOrganisationOrganisationClassification>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public UpdateOrganisationOrganisationClassificationCommandHandler(ILogger<UpdateOrganisationOrganisationClassificationCommandHandler> logger, ISession session, IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationOrganisationClassification> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .WithOrganisationClassificationTypePolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var organisationClassification =
                        session.Get<OrganisationClassification>(envelope.Command.OrganisationClassificationId);
                    var organisationClassificationType =
                        session.Get<OrganisationClassificationType>(envelope.Command.OrganisationClassificationTypeId);

                    KboV2Guards.ThrowIfLegalForm(_organisationRegistryConfiguration, organisationClassificationType);

                    organisation.UpdateOrganisationClassification(
                        envelope.Command.OrganisationOrganisationClassificationId,
                        organisationClassificationType,
                        organisationClassification,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
