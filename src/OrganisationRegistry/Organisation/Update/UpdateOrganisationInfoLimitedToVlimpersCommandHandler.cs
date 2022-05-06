namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationInfoLimitedToVlimpersCommandHandler :
    BaseCommandHandler<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationInfoLimitedToVlimpers>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationInfoLimitedToVlimpersCommandHandler(
        ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationInfoLimitedToVlimpers> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithVlimpersOnlyPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateVlimpersOrganisationInfo(
                        envelope.Command.Article,
                        envelope.Command.Name,
                        envelope.Command.ShortName,
                        new Period(
                            envelope.Command.ValidFrom,
                            envelope.Command.ValidTo),
                        new Period(
                            envelope.Command.OperationalValidFrom,
                            envelope.Command.OperationalValidTo),
                        _dateTimeProvider);
                });
}
