namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveOrganisationBankAccountCommandHandler
    : BaseCommandHandler<RemoveOrganisationBankAccountCommandHandler>
        , ICommandEnvelopeHandler<RemoveOrganisationBankAccount>
{
    public RemoveOrganisationBankAccountCommandHandler(ILogger<RemoveOrganisationBankAccountCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<RemoveOrganisationBankAccount> envelope)
        => await UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresAdmin()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.Id);
                    organisation.RemoveBankAccount(envelope.Command.OrganisationBankAccountId);
                });
}
