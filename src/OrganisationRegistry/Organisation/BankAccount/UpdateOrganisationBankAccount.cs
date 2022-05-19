namespace OrganisationRegistry.Organisation;

using System;

public class UpdateOrganisationBankAccount : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationBankAccountId { get; }
    public string BankAccountNumber { get; }
    public bool IsIban { get; }
    public string? Bic { get; }
    public bool IsBic { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationBankAccount(
        Guid organisationBankAccountId,
        OrganisationId organisationId,
        string bankAccountNumber,
        bool isIban,
        string? bic,
        bool isBic,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationBankAccountId = organisationBankAccountId;
        BankAccountNumber = bankAccountNumber;
        IsIban = isIban;
        ValidFrom = validFrom;
        ValidTo = validTo;
        Bic = bic;
        IsBic = isBic;
    }
}
