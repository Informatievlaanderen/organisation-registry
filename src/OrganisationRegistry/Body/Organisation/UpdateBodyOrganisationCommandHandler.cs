namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class UpdateBodyOrganisationCommandHandler
    : BaseCommandHandler<UpdateBodyOrganisationCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyOrganisation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateBodyOrganisationCommandHandler(
        ILogger<UpdateBodyOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyOrganisation> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateOrganisation(
                        envelope.Command.BodyOrganisationId,
                        organisation,
                        envelope.Command.Validity,
                        _dateTimeProvider);
                });
}
