namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationBankAccountUpdated : BaseEvent<OrganisationBankAccountUpdated>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationBankAccountId { get; }

        public string BankAccountNumber { get; }
        public string PreviousBankAccountNumber { get; }

        public string Bic { get; }
        public string PreviousBic { get; }

        public bool IsIban { get; }
        public bool WasPreviouslyIban { get; }

        public bool IsBic { get; }
        public bool WasPreviouslyBic { get; }

        public DateTime? ValidFrom { get; }
        public DateTime? PreviouslyValidFrom { get; }

        public DateTime? ValidTo { get; }
        public DateTime? PreviouslyValidTo { get; }

        public OrganisationBankAccountUpdated(
            Guid organisationId,
            Guid organisationBankAccountId,
            string bankAccountNumber,
            bool isIban,
            string bic,
            bool isBic,
            DateTime? validFrom,
            DateTime? validTo,
            string previousBankAccountNumber,
            bool wasPreviouslyIban,
            string previousBic,
            bool wasPreviouslyBic,
            DateTime? previouslyValidFrom,
            DateTime? previouslyValidTo)
        {
            Id = organisationId;

            OrganisationBankAccountId = organisationBankAccountId;
            BankAccountNumber = bankAccountNumber;
            IsIban = isIban;
            Bic = bic;
            IsBic = isBic;
            ValidFrom = validFrom;
            ValidTo = validTo;

            PreviousBankAccountNumber = previousBankAccountNumber;
            WasPreviouslyIban = wasPreviouslyIban;
            PreviousBic = previousBic;
            WasPreviouslyBic = wasPreviouslyBic;
            PreviouslyValidFrom = previouslyValidFrom;
            PreviouslyValidTo = previouslyValidTo;
        }
    }
}
