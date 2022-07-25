namespace OrganisationRegistry.Organisation;

using System.Collections.Generic;
using System.Linq;

public class OrganisationBankAccounts : OrganisationList<OrganisationBankAccount>
{
    public OrganisationBankAccounts()
    {
    }

    public OrganisationBankAccounts(IEnumerable<OrganisationBankAccount> organisationBankAccounts) : base(organisationBankAccounts)
    {
    }

    public OrganisationBankAccounts(params OrganisationBankAccount[] organisationBankAccounts) : base(organisationBankAccounts)
    {
    }

    public bool HasBankAccountNumbersThatWouldOverlapWith(OrganisationBankAccount organisationBankAccount)
        => WithBankAccount(organisationBankAccount.BankAccountNumber)
            .Except(organisationBankAccount.OrganisationBankAccountId)
            .OverlappingWith(organisationBankAccount.Validity)
            .Any();

    public OrganisationBankAccounts WithBankAccount(string bankAccountNumber)
        => new(
            this.Where(ob => ob.BankAccountNumber == bankAccountNumber));
}
