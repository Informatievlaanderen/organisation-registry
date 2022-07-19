namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;

public class OrganisationBankAccounts : List<OrganisationBankAccount>
{
    public OrganisationBankAccounts()
    {
    }

    public OrganisationBankAccounts(IEnumerable<OrganisationBankAccount> organisationBuildings) : base(organisationBuildings)
    {
    }

    public OrganisationBankAccounts(params OrganisationBankAccount[] organisationBuildings) : base(organisationBuildings)
    {
    }

    public bool HasBankAccountNumbersThatWouldOverlapWith(OrganisationBankAccount organisationBankAccount)
        => Except(organisationBankAccount.OrganisationBankAccountId)
            .WithBankAccount(organisationBankAccount.BankAccountNumber)
            .OverlappingWith(organisationBankAccount.Validity)
            .Any();

    public OrganisationBankAccounts Except(Guid organisationBankAccountId)
        => new(
            this.Where(ob => ob.OrganisationBankAccountId != organisationBankAccountId));

    public OrganisationBankAccounts WithBankAccount(string bankAccountNumber)
        => new(
            this.Where(ob => ob.BankAccountNumber == bankAccountNumber));

    public OrganisationBankAccounts OverlappingWith(Period validity)
        => new(
            this.Where(ob => ob.Validity.OverlapsWith(validity)));

    public OrganisationBankAccount? this[Guid id]
        => this.SingleOrDefault(ob => ob.Id == id);

}
