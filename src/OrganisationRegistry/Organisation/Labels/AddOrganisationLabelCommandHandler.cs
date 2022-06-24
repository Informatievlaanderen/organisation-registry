namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using LabelType;
using Microsoft.Extensions.Logging;

public class AddOrganisationLabelCommandHandler
    : BaseCommandHandler<AddOrganisationLabelCommandHandler>
        , ICommandEnvelopeHandler<AddOrganisationLabel>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public AddOrganisationLabelCommandHandler(
        ILogger<AddOrganisationLabelCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationLabel> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithLabelPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    var labelType = session.Get<LabelType>(envelope.Command.LabelTypeId);

                    organisation.AddLabel(
                        _organisationRegistryConfiguration.Kbo,
                        envelope.User,
                        envelope.Command.OrganisationLabelId,
                        labelType,
                        envelope.Command.LabelValue,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
