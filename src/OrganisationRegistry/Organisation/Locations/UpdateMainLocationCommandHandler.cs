namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateMainLocationCommandHandler : BaseCommandHandler<UpdateMainLocationCommandHandler>,
    ICommandEnvelopeHandler<UpdateMainLocation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateMainLocationCommandHandler(
        ILogger<UpdateMainLocationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(
        logger,
        session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateMainLocation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateMainLocation(_dateTimeProvider.Today);
                });
}
