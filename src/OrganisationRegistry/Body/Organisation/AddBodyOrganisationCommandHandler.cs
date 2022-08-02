namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class AddBodyOrganisationCommandHandler
    : BaseCommandHandler<AddBodyOrganisationCommandHandler>,
        ICommandEnvelopeHandler<AddBodyOrganisation>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddBodyOrganisationCommandHandler(
        ILogger<AddBodyOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ICommandEnvelope<AddBodyOrganisation> envelope)
        => await UpdateHandler<Body>
            .For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.AddOrganisation(
                        envelope.Command.BodyOrganisationId,
                        organisation,
                        envelope.Command.Validity,
                        _dateTimeProvider);
                });
}
