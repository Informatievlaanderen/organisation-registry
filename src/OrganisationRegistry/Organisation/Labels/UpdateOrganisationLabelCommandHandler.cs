namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using LabelType;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationLabelCommandHandler
    : BaseCommandHandler<UpdateOrganisationLabelCommandHandler>
        , ICommandEnvelopeHandler<UpdateOrganisationLabel>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public UpdateOrganisationLabelCommandHandler(ILogger<UpdateOrganisationLabelCommandHandler> logger, ISession session, IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationLabel> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .WithLabelPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var labelType = session.Get<LabelType>(envelope.Command.LabelTypeId);

                    KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

                    organisation.UpdateLabel(
                        envelope.Command.OrganisationLabelId,
                        labelType,
                        envelope.Command.Value,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
