namespace OrganisationRegistry.Organisation.Events;

using System;

public class KboOrganisationBankAccountAdded : BaseEvent<KboOrganisationBankAccountAdded>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationBankAccountId { get; }
    public string BankAccountNumber { get; }
    public bool IsIban { get; }
    public string Bic { get; }
    public bool IsBic { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public KboOrganisationBankAccountAdded(
        Guid organisationId,
        Guid organisationBankAccountId,
        string bankAccountNumber,
        bool isIban,
        string bic,
        bool isBic,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationBankAccountId = organisationBankAccountId;
        BankAccountNumber = bankAccountNumber;
        IsIban = isIban;
        Bic = bic;
        IsBic = isBic;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public static KboOrganisationBankAccountAdded From(
        Guid organisationId,
        Guid organisationBankAccountId,
        BankAccountNumber bankAccountNumber,
        BankAccountBic bankAccountBic,
        Period validity)
        => new(
            organisationId,
            organisationBankAccountId,
            bankAccountNumber.Number,
            bankAccountNumber.IsValidIban,
            bankAccountBic.Bic,
            bankAccountBic.IsValidBic,
            validity.Start,
            validity.End);
}
