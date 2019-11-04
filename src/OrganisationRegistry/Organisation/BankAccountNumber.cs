namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Text.RegularExpressions;
    using IbanBic;

    public class BankAccountNumber
    {
        public string Number { get; }
        public bool IsValidIban { get; }

        public BankAccountNumber(
            string bankAccountNumber,
            bool isValidIban)
        {
            IsValidIban = isValidIban;
            Number = isValidIban ? CleanBankAccountNumber(bankAccountNumber) : bankAccountNumber;

            if (isValidIban)
                ValidateIbanOrThrow();
        }

        private static string CleanBankAccountNumber(string bankAccountNumber)
            => Regex.Replace(bankAccountNumber, @"[^0-9a-zA-Z]", "");

        private void ValidateIbanOrThrow()
        {
            try
            {
                IbanUtils.Validate(Number);
            }
            catch (Exception ex)
            {
                throw new InvalidIbanException(ex);
            }
        }
    }
}
