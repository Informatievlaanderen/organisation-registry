namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Commands;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationOpeningHourCommandHandler
    : BaseCommandHandler<UpdateOrganisationOpeningHourCommandHandler>,
        ICommandEnvelopeHandler<UpdateOrganisationOpeningHour>
{
    public UpdateOrganisationOpeningHourCommandHandler(ILogger<UpdateOrganisationOpeningHourCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationOpeningHour> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateOpeningHour(
                        envelope.Command.OrganisationOpeningHourId,
                        envelope.Command.Opens,
                        envelope.Command.Closes,
                        envelope.Command.DayOfWeek,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
