namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddOrganisationOpeningHourCommandHandler
    : BaseCommandHandler<AddOrganisationOpeningHourCommandHandler>,
        ICommandEnvelopeHandler<AddOrganisationOpeningHour>
{
    public AddOrganisationOpeningHourCommandHandler(ILogger<AddOrganisationOpeningHourCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationOpeningHour> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithBeheerderForOrganisationPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.AddOpeningHour(
                        envelope.Command.OrganisationOpeningHourId,
                        envelope.Command.Opens,
                        envelope.Command.Closes,
                        envelope.Command.DayOfWeek,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
