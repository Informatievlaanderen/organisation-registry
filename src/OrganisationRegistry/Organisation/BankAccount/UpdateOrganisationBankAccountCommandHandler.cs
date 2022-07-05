namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationBankAccountCommandHandler :
    BaseCommandHandler<UpdateOrganisationBankAccountCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationBankAccount>
{
    public UpdateOrganisationBankAccountCommandHandler(ILogger<UpdateOrganisationBankAccountCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationBankAccount> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var bankAccountNumber = BankAccountNumber.CreateWithExpectedValidity(
                        envelope.Command.BankAccountNumber,
                        envelope.Command.IsIban);
                    var bankAccountBic = BankAccountBic.CreateWithExpectedValidity(envelope.Command.Bic, envelope.Command.IsBic);

                    var validity = new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo));

                    organisation.UpdateBankAccount(
                        envelope.Command.OrganisationBankAccountId,
                        bankAccountNumber,
                        bankAccountBic,
                        validity);
                });
}
