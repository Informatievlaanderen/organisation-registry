namespace OrganisationRegistry.Organisation;

using System.Linq;
using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Purpose;

public class UpdateOrganisationNotLimitedToVlimpersCommandHandler :
    BaseCommandHandler<UpdateOrganisationNotLimitedToVlimpersCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationInfoNotLimitedToVlimpers>
{
    public UpdateOrganisationNotLimitedToVlimpersCommandHandler(
        ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationInfoNotLimitedToVlimpers> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var purposes = envelope.Command
                        .Purposes
                        .Select(purposeId => session.Get<Purpose>(purposeId))
                        .ToList();

                    organisation.UpdateInfoNotLimitedByVlimpers(
                        envelope.Command.Description,
                        purposes,
                        envelope.Command.ShowOnVlaamseOverheidSites);
                });
}
