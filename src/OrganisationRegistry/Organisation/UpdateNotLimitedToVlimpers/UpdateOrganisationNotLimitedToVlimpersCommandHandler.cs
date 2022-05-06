namespace OrganisationRegistry.Organisation.UpdateNotLimitedToVlimpers;

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
    public UpdateOrganisationNotLimitedToVlimpersCommandHandler(ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationInfoNotLimitedToVlimpers> envelope)
        => Handle(envelope.Command, envelope.User);

    public Task Handle(UpdateOrganisationInfoNotLimitedToVlimpers message, IUser user) =>
        UpdateHandler<Organisation>.For(message, user, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(session =>
            {
                var organisation = session.Get<Organisation>(message.OrganisationId);
                organisation.ThrowIfTerminated(user);

                var purposes = message
                    .Purposes
                    .Select(purposeId => session.Get<Purpose>(purposeId))
                    .ToList();

                organisation.UpdateInfoNotLimitedByVlimpers(
                    message.Description,
                    purposes,
                    message.ShowOnVlaamseOverheidSites);
            });
}
