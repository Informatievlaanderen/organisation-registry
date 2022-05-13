namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using OrganisationClassification;
using OrganisationClassificationType;

public class AddOrganisationOrganisationClassificationCommandHandler
    : BaseCommandHandler<AddOrganisationOrganisationClassificationCommandHandler>
        , ICommandEnvelopeHandler<AddOrganisationOrganisationClassification>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public AddOrganisationOrganisationClassificationCommandHandler(
        ILogger<AddOrganisationOrganisationClassificationCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationOrganisationClassification> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
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

                    organisation.AddOrganisationClassification(
                        _organisationRegistryConfiguration,
                        envelope.Command.OrganisationOrganisationClassificationId,
                        organisationClassificationType,
                        organisationClassification,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
