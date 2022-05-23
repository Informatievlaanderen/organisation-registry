namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationFormalFrameworkParentsCommandHandler
    : BaseCommandHandler<UpdateOrganisationFormalFrameworkParentsCommandHandler>,
        ICommandEnvelopeHandler<UpdateOrganisationFormalFrameworkParents>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationFormalFrameworkParentsCommandHandler(
        ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationFormalFrameworkParents> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateOrganisationFormalFrameworkParent(
                        _dateTimeProvider.Today,
                        envelope.Command.FormalFrameworkId);
                });
}
