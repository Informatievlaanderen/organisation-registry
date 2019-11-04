namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationBankAccount
    {
        public Guid OrganisationId { get; }
        public Guid OrganisationBankAccountId { get; }
        public string BankAccountNumber { get; }
        public bool IsIban { get; }
        public string Bic { get; }
        public bool IsBic { get; }
        public Period Validity { get; }

        public OrganisationBankAccount(
            Guid organisationBankAccountId,
            Guid organisationId,
            string bankAccountNumber,
            bool isIban,
            string bic,
            bool isBic,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationBankAccountId = organisationBankAccountId;
            BankAccountNumber = bankAccountNumber;
            IsIban = isIban;
            Bic = bic;
            IsBic = isBic;
            Validity = validity;
        }
    }
}
