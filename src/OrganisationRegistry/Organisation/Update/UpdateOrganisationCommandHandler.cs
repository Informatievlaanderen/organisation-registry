namespace OrganisationRegistry.Organisation.Update;

using System.Linq;
using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Purpose;

public class UpdateOrganisationCommandHandler:
    BaseCommandHandler<UpdateOrganisationCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationInfo>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationCommandHandler(
        ILogger<UpdateOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationInfo> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresBeheerderForOrganisationButNotUnderVlimpersManagement()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var purposes = envelope.Command
                        .Purposes
                        .Select(purposeId => session.Get<Purpose>(purposeId))
                        .ToList();

                    organisation.UpdateInfo(
                        envelope.Command.Name,
                        envelope.Command.Article,
                        envelope.Command.Description,
                        envelope.Command.ShortName,
                        purposes,
                        envelope.Command.ShowOnVlaamseOverheidSites,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        new Period(
                            new ValidFrom(envelope.Command.OperationalValidFrom),
                            new ValidTo(envelope.Command.OperationalValidTo)),
                        _dateTimeProvider);
                });
}
